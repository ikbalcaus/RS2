import "dart:io";
import "package:ebooks_user/models/access_rights/access_right.dart";
import "package:ebooks_user/models/books/book.dart";
import "package:ebooks_user/models/reviews/review.dart";
import "package:ebooks_user/models/search_result.dart";
import "package:ebooks_user/providers/access_rights_provider.dart";
import "package:ebooks_user/providers/auth_provider.dart";
import "package:ebooks_user/providers/books_provider.dart";
import "package:ebooks_user/providers/publisher_follows_provider.dart";
import "package:ebooks_user/providers/reports_provider.dart";
import "package:ebooks_user/providers/reviews_provider.dart";
import "package:ebooks_user/providers/stripe_provider.dart";
import "package:ebooks_user/providers/wishlist_provider.dart";
import "package:ebooks_user/screens/books_screen.dart";
import "package:ebooks_user/screens/edit_book_screen.dart";
import "package:ebooks_user/screens/master_screen.dart";
import "package:ebooks_user/screens/pdf_screen.dart";
import "package:ebooks_user/screens/profile_screen.dart";
import "package:ebooks_user/screens/publisher_screen.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:ebooks_user/utils/helpers.dart";
import "package:flutter/gestures.dart";
import "package:flutter/material.dart";
import "package:path_provider/path_provider.dart";
import "package:provider/provider.dart";
import "package:url_launcher/url_launcher.dart";
import 'package:easy_localization/easy_localization.dart';

class BookDetailsScreen extends StatefulWidget {
  final int bookId;
  const BookDetailsScreen({super.key, required this.bookId});

  @override
  State<BookDetailsScreen> createState() => _BookDetailsScreenState();
}

class _BookDetailsScreenState extends State<BookDetailsScreen> {
  late BooksProvider _booksProvider;
  late AccessRightsProvider _accessRightsProvider;
  late WishlistProvider _wishlistProvider;
  late PublisherFollowsProvider _publisherFollowsProvider;
  late ReportsProvider _reportsProvider;
  late ReviewsProvider _reviewsProvider;
  late StripeProvider _stripeProvider;
  Book? _book;
  List<Book>? _recommendedBooks;
  SearchResult<Review>? _reviews;
  AccessRight? _accessRight;
  final List<String> _allowedActions = [];
  int _reviewsPage = 1;
  Map<String, dynamic> _reviewsFilter = {};
  bool _isLoading = true;
  final Map<String, Future<void> Function()> _popupActions = {};
  final ScrollController _scrollController = ScrollController();

  @override
  void initState() {
    super.initState();
    _booksProvider = context.read<BooksProvider>();
    _accessRightsProvider = context.read<AccessRightsProvider>();
    _wishlistProvider = context.read<WishlistProvider>();
    _publisherFollowsProvider = context.read<PublisherFollowsProvider>();
    _reportsProvider = context.read<ReportsProvider>();
    _reviewsProvider = context.read<ReviewsProvider>();
    _stripeProvider = context.read<StripeProvider>();
    _reviewsFilter = {"BookId": widget.bookId};
    _fetchBook();
    _fetchRecommendedBooks();
    _fetchReviews();
    _fetchAccessRight();
    _scrollController.addListener(() {
      if (_scrollController.position.pixels >=
          _scrollController.position.maxScrollExtent) {
        if (!_isLoading &&
            (_reviews?.resultList.length ?? 0) < (_reviews?.count ?? 0)) {
          _reviewsPage++;
          _fetchReviews(append: true);
        }
      }
    });
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    if (_isLoading) {
      return MasterScreen(
        child: const Center(child: CircularProgressIndicator()),
      );
    }
    return MasterScreen(
      showBackButton: true,
      popupActions: _popupActions,
      child: _buildResultView(),
    );
  }

  Future _fetchBook() async {
    setState(() => _isLoading = true);
    try {
      final book = await _booksProvider.getById(widget.bookId);
      if (!mounted) return;
      setState(() => _book = book);
    } catch (ex) {
      if (!mounted) return;
      Helpers.showErrorMessage(context, ex);
    } finally {
      if (!mounted) return;
      setState(() => _isLoading = false);
      await _getAllowedActions();
    }
  }

  Future _fetchRecommendedBooks() async {
    try {
      final recommendedBooks = await _booksProvider.getRecommendedBooks(
        widget.bookId,
      );
      if (!mounted) return;
      setState(() => _recommendedBooks = recommendedBooks);
    } catch (ex) {
      if (!mounted) return;
      Helpers.showErrorMessage(context, ex);
    }
  }

  Future _fetchReviews({bool append = false}) async {
    try {
      final reviews = await _reviewsProvider.getPaged(
        page: _reviewsPage,
        filter: _reviewsFilter,
      );
      if (!mounted) return;
      setState(() {
        if (append && _reviews != null) {
          _reviews?.resultList.addAll(reviews.resultList);
          _reviews?.count = reviews.count;
        } else {
          _reviews = reviews;
        }
      });
    } catch (ex) {
      if (!mounted) return;
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (!mounted) return;
        Helpers.showErrorMessage(context, ex);
      });
    }
  }

  Future _fetchAccessRight() async {
    try {
      final accessRight = await _accessRightsProvider.getById(widget.bookId);
      if (!mounted) return;
      setState(() => _accessRight = accessRight);
    } catch (ex) {
      if (!mounted) return;
      setState(() => _accessRight = null);
    } finally {
      if (!mounted) return;
      setState(() => _isLoading = false);
    }
  }

  Future _getAllowedActions() async {
    try {
      List<String> allowedActions = [];
      if (_book?.publisher?.userId == AuthProvider.userId) {
        allowedActions = await _booksProvider.getAllowedActions(widget.bookId);
      }
      if (!mounted) return;
      setState(() {
        _allowedActions
          ..clear()
          ..addAll(allowedActions);
        _popupActions.clear();
        _popupActions["Add to Wishlist".tr()] = () async {
          try {
            await _wishlistProvider.post(null, _book?.bookId);
            Helpers.showSuccessMessage(
              context,
              "Book is added to wishlist".tr(),
            );
          } catch (ex) {
            AuthProvider.isLoggedIn
                ? Helpers.showErrorMessage(
                    context,
                    "Book is already in your wishlist".tr(),
                  )
                : Helpers.showErrorMessage(
                    context,
                    "You must be logged in".tr(),
                  );
          }
        };
        _popupActions["Follow Publisher".tr()] = () async {
          try {
            await _publisherFollowsProvider.post(
              null,
              _book?.publisher?.userId,
            );
            Helpers.showSuccessMessage(
              context,
              "You are now following ${_book?.publisher?.userName}".tr(),
            );
          } catch (ex) {
            AuthProvider.isLoggedIn
                ? Helpers.showErrorMessage(
                    context,
                    "You already follow ${_book?.publisher?.userName}".tr(),
                  )
                : Helpers.showErrorMessage(
                    context,
                    "You must be logged in".tr(),
                  );
          }
        };
        if (_accessRight != null &&
            _book?.publisher?.userId != AuthProvider.userId) {
          _popupActions["Review Book".tr()] = () async =>
              await _showReviewBookDialog();
        }
        if (_allowedActions.contains("Update")) {
          _popupActions["Edit Book".tr()] = () async => Navigator.push(
            context,
            MaterialPageRoute(
              builder: (context) => EditBookScreen(bookId: _book!.bookId!),
            ),
          );
        }
        if (_allowedActions.contains("Await")) {
          _popupActions["Publish Book".tr()] = () async =>
              await _showAwaitBookDialog();
        }
        if (_allowedActions.contains("Hide")) {
          _popupActions[_book?.status == "Approved"
              ? "Hide Book".tr()
              : "Unhide Book".tr()] = () async =>
              await _showHideBookDialog();
        }
        if (AuthProvider.userId == _book!.publisher!.userId) {
          _popupActions["Set Discount".tr()] = () async =>
              await _showSetDiscountDialog();
        }
        _popupActions["Report Book".tr()] = () async {
          AuthProvider.isLoggedIn
              ? await _showReportBookDialog()
              : Helpers.showErrorMessage(context, "You must be logged in".tr());
        };
      });
    } catch (ex) {
      if (!mounted) return;
      Helpers.showErrorMessage(context, ex);
    }
  }

  Future _openBookFile(String fileName, int bookId) async {
    try {
      final dir = await getApplicationDocumentsDirectory();
      final filePath = "${dir.path}/$fileName.pdf";
      final file = File(filePath);
      if (!await file.exists()) {
        Helpers.showSuccessMessage(context, "Book is downloading...".tr());
        await _booksProvider.getBookFile(bookId, filePath);
      }
      Navigator.push(
        context,
        MaterialPageRoute(
          builder: (context) => PdfScreen(bookId: bookId, filePath: filePath),
        ),
      );
    } catch (ex) {
      if (!mounted) return;
      Helpers.showErrorMessage(context, ex);
    }
  }

  Future _openStripePaymentPage() async {
    try {
      var paymentPageLink = await _stripeProvider.getPaymentPageLink(
        _book!.bookId!,
      );
      final uri = Uri.parse(paymentPageLink.url);
      if (await canLaunchUrl(uri)) {
        await launchUrl(uri, mode: LaunchMode.externalApplication);
      } else {
        Helpers.showErrorMessage(
          context,
          "Cannot open an URL: ${uri.toString()}".tr(),
        );
      }
    } catch (ex) {
      Helpers.showErrorMessage(context, ex);
    }
  }

  Future _showReviewBookDialog() async {
    int rating = 0;
    String comment = "";
    await showDialog(
      context: context,
      builder: (context) {
        return StatefulBuilder(
          builder: (context, setState) {
            return AlertDialog(
              title: const Text("Confirm review").tr(),
              content: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: List.generate(5, (index) {
                      return IconButton(
                        icon: Icon(
                          index < rating ? Icons.star : Icons.star_border,
                          color: Colors.orange,
                        ),
                        onPressed: () => setState(() => rating = index + 1),
                      );
                    }),
                  ),
                  TextField(
                    decoration: InputDecoration(labelText: "Comment...".tr()),
                    onChanged: (value) => comment = value,
                  ),
                ],
              ),
              actions: [
                TextButton(
                  onPressed: () async {
                    if (rating >= 1 && rating <= 5) {
                      try {
                        if (!(_reviews?.resultList.any(
                              (review) =>
                                  review.user?.userId == AuthProvider.userId,
                            ) ??
                            false)) {
                          await _reviewsProvider.post({
                            "rating": rating,
                            "comment": comment,
                          }, widget.bookId);
                        } else {
                          await _reviewsProvider.put(widget.bookId, {
                            "rating": rating,
                            "comment": comment,
                          });
                        }
                        _fetchReviews();
                        if (context.mounted) {
                          Navigator.pop(context);
                          Helpers.showSuccessMessage(
                            context,
                            "Book reviewed successfully".tr(),
                          );
                        }
                      } catch (ex) {
                        Navigator.pop(context);
                        Helpers.showErrorMessage(context, ex);
                      }
                    }
                  },
                  child: const Text("Submit").tr(),
                ),
                TextButton(
                  onPressed: () => Navigator.pop(context),
                  child: const Text("Cancel").tr(),
                ),
              ],
            );
          },
        );
      },
    );
  }

  Future _showDeleteReviewDialog() async {
    await showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text("Delete review").tr(),
        content: const Text("Are you sure you want to delete review?").tr(),
        actions: [
          TextButton(
            onPressed: () async {
              try {
                await _reviewsProvider.delete(_book!.bookId!);
                await _fetchReviews();
                if (context.mounted) {
                  Navigator.pop(context);
                  Helpers.showSuccessMessage(
                    context,
                    "Successfully deleted review".tr(),
                  );
                }
              } catch (ex) {
                Navigator.pop(context);
                Helpers.showErrorMessage(context, ex);
              }
            },
            child: const Text("Delete").tr(),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text("Cancel").tr(),
          ),
        ],
      ),
    );
  }

  Future _showSetDiscountDialog() async {
    final percentageController = TextEditingController();
    DateTime? startDate;
    DateTime? endDate;
    await showDialog(
      context: context,
      builder: (context) {
        return StatefulBuilder(
          builder: (context, setState) {
            Future pickDate(bool isStart) async {
              final picked = await showDatePicker(
                context: context,
                initialDate: DateTime.now(),
                firstDate: DateTime(2020),
                lastDate: DateTime(2100),
              );
              if (picked != null) {
                setState(() => isStart ? startDate = picked : endDate = picked);
              }
            }

            return AlertDialog(
              title: const Text("Set Discount").tr(),
              content: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  TextField(
                    controller: percentageController,
                    decoration: InputDecoration(
                      labelText: "Discount % (0-100)".tr(),
                    ),
                    keyboardType: TextInputType.number,
                  ),
                  Row(
                    children: [
                      Expanded(child: Text("Start: ").tr()),
                      TextButton(
                        onPressed: () => pickDate(true),
                        child: const Text("Pick").tr(),
                      ),
                    ],
                  ),
                  Row(
                    children: [
                      Expanded(child: Text("End: ").tr()),
                      TextButton(
                        onPressed: () => pickDate(false),
                        child: const Text("Pick").tr(),
                      ),
                    ],
                  ),
                ],
              ),
              actions: [
                TextButton(
                  onPressed: () async {
                    final percentage = int.tryParse(percentageController.text);
                    if (percentage != null &&
                        percentage >= 0 &&
                        percentage <= 100 &&
                        startDate != null &&
                        endDate != null &&
                        endDate!.isAfter(startDate!)) {
                      try {
                        await _booksProvider.setDiscount(widget.bookId, {
                          "discountPercentage": percentage,
                          "discountStart": startDate!.toIso8601String(),
                          "discountEnd": endDate!.toIso8601String(),
                        });
                        if (context.mounted) {
                          Navigator.pop(context);
                          Helpers.showSuccessMessage(
                            context,
                            "Discount set successfully".tr(),
                          );
                        }
                      } catch (ex) {
                        Navigator.pop(context);
                        Helpers.showErrorMessage(context, ex);
                      }
                    } else {
                      Helpers.showErrorMessage(context, "Invalid input".tr());
                    }
                  },
                  child: const Text("Save").tr(),
                ),
                TextButton(
                  onPressed: () => Navigator.pop(context),
                  child: const Text("Cancel").tr(),
                ),
              ],
            );
          },
        );
      },
    );
  }

  Future _showReportBookDialog() async {
    String reason = "";
    await showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: const Text("Confirm report").tr(),
          content: TextField(
            decoration: InputDecoration(labelText: "Enter reason...".tr()),
            onChanged: (value) => reason = value,
          ),
          actions: [
            TextButton(
              onPressed: () async {
                if (reason.trim().isNotEmpty) {
                  try {
                    await _reportsProvider.post({
                      "reason": reason,
                    }, widget.bookId);
                    if (context.mounted) {
                      Navigator.pop(context);
                      Helpers.showSuccessMessage(
                        context,
                        "Successfully reported book".tr(),
                      );
                    }
                  } catch (ex) {
                    Navigator.pop(context);
                    Helpers.showErrorMessage(
                      context,
                      "You already reported this book".tr(),
                    );
                  }
                }
              },
              child: const Text("Report").tr(),
            ),
            TextButton(
              onPressed: () => Navigator.pop(context),
              child: const Text("Cancel").tr(),
            ),
          ],
        );
      },
    );
  }

  Future _showReportReviewDialog(int userId) async {
    String reason = "";
    await showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: const Text("Confirm report").tr(),
          content: TextField(
            decoration: InputDecoration(labelText: "Reason...".tr()),
            onChanged: (value) => reason = value,
          ),
          actions: [
            TextButton(
              onPressed: () async {
                if (reason.trim().isNotEmpty) {
                  try {
                    await _reviewsProvider.report(
                      userId,
                      widget.bookId,
                      reason,
                    );
                    if (context.mounted) {
                      Navigator.pop(context);
                      Helpers.showSuccessMessage(
                        context,
                        "Successfully reported review".tr(),
                      );
                    }
                  } catch (ex) {
                    Navigator.pop(context);
                    Helpers.showErrorMessage(context, ex);
                  }
                }
              },
              child: const Text("Report").tr(),
            ),
            TextButton(
              onPressed: () => Navigator.pop(context),
              child: const Text("Cancel").tr(),
            ),
          ],
        );
      },
    );
  }

  Future _showAwaitBookDialog() async {
    await showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text("Publish book").tr(),
        content: const Text("Are you sure you want to await this book?").tr(),
        actions: [
          TextButton(
            onPressed: () async {
              try {
                await _booksProvider.awaitBook(_book!.bookId!);
                Helpers.showSuccessMessage(
                  context,
                  "Book is now pending review by moderators".tr(),
                );
                _fetchBook();
                Navigator.pushAndRemoveUntil(
                  context,
                  MaterialPageRoute(builder: (context) => ProfileScreen()),
                  (_) => false,
                );
              } catch (ex) {
                Helpers.showErrorMessage(context, ex);
              }
            },
            child: const Text("Publish").tr(),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text("Cancel").tr(),
          ),
        ],
      ),
    );
  }

  Future _showHideBookDialog() async {
    await showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: _book?.status == "Approved"
            ? const Text("Hide book").tr()
            : const Text("Unhide book").tr(),
        content: const Text("Are you sure you want to hide this book?").tr(),
        actions: [
          TextButton(
            onPressed: () async {
              try {
                await _booksProvider.hideBook(_book!.bookId!);
                Helpers.showSuccessMessage(
                  context,
                  _book?.status == "Approved"
                      ? "Book is now hidden to other users".tr()
                      : "Book is now available to other users".tr(),
                );
                _fetchBook();
                Navigator.pushAndRemoveUntil(
                  context,
                  MaterialPageRoute(builder: (context) => ProfileScreen()),
                  (_) => false,
                );
              } catch (ex) {
                Helpers.showErrorMessage(context, ex);
              }
            },
            child: _book?.status == "Approved"
                ? const Text("Hide").tr()
                : const Text("Unhide").tr(),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text("Cancel").tr(),
          ),
        ],
      ),
    );
  }

  Widget _buildResultView() {
    return SingleChildScrollView(
      controller: _scrollController,
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Center(
            child: ClipRRect(
              borderRadius: BorderRadius.circular(10),
              child: Image.network(
                "${Globals.apiAddress}/images/books/${_book?.filePath}.webp?t=${DateTime.now().millisecondsSinceEpoch}",
                width: 300,
                height: 450,
                fit: BoxFit.cover,
                errorBuilder: (context, error, stackTrace) =>
                    const Icon(Icons.broken_image, size: 300),
              ),
            ),
          ),
          const SizedBox(height: 16),
          Text(
            _book?.title ?? "",
            style: const TextStyle(fontSize: 24, fontWeight: FontWeight.w700),
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
          ),
          const SizedBox(height: 18),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 24),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Column(
                  children: [
                    const Text("Views").tr(),
                    const SizedBox(height: 4),
                    Text(
                      (_book?.numberOfViews?.toString() ?? ""),
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                  ],
                ),
                Column(
                  children: [
                    Text("Rating").tr(),
                    SizedBox(height: 4),
                    Text(
                      (_book?.averageRating == 0.0)
                          ? "N/A".tr()
                          : (_book?.averageRating ?? "").toString(),
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                  ],
                ),
                Column(
                  children: [
                    const Text("Language").tr(),
                    const SizedBox(height: 4),
                    Text(
                      _book?.language?.name ?? "N/A".tr(),
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                  ],
                ),
                Column(
                  children: [
                    const Text("Pages").tr(),
                    const SizedBox(height: 4),
                    Text(
                      (_book?.numberOfPages?.toString() ?? ""),
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),
          const SizedBox(height: 24),
          RichText(
            text: TextSpan(
              style: TextStyle(
                color: Theme.of(context).textTheme.bodyMedium?.color,
                fontSize: 14,
              ),
              children: [
                TextSpan(
                  text: (_book?.authors?.length ?? 1) == 1
                      ? "Author: ".tr()
                      : "Authors: ".tr(),
                  style: const TextStyle(fontWeight: FontWeight.w700),
                ),
                ...?_book?.authors?.asMap().entries.expand((entry) {
                  final index = entry.key;
                  final author = entry.value;
                  final isLast = index == _book!.authors!.length - 1;
                  return [
                    TextSpan(
                      text: author.name ?? "",
                      style: TextStyle(
                        color: Globals.backgroundColor,
                        fontWeight: FontWeight.w500,
                      ),
                      recognizer: TapGestureRecognizer()
                        ..onTap = () => Navigator.pushAndRemoveUntil(
                          context,
                          MaterialPageRoute(
                            builder: (context) =>
                                BooksScreen(author: author.name),
                          ),
                          (_) => false,
                        ),
                    ),
                    if (!isLast) const TextSpan(text: ", "),
                  ];
                }),
              ],
            ),
          ),
          const SizedBox(height: 4),
          RichText(
            text: TextSpan(
              style: TextStyle(
                color: Theme.of(context).textTheme.bodyMedium?.color,
                fontSize: 14,
              ),
              children: [
                TextSpan(
                  text: (_book?.genres?.length ?? 1) == 1
                      ? "Genre: ".tr()
                      : "Genres: ".tr(),
                  style: const TextStyle(fontWeight: FontWeight.w700),
                ),
                ...?_book?.genres?.asMap().entries.expand((entry) {
                  final index = entry.key;
                  final genre = entry.value;
                  final isLast = index == _book!.genres!.length - 1;
                  return [
                    TextSpan(
                      text: genre.name ?? "",
                      style: TextStyle(
                        color: Globals.backgroundColor,
                        fontWeight: FontWeight.w500,
                      ),
                      recognizer: TapGestureRecognizer()
                        ..onTap = () => Navigator.pushAndRemoveUntil(
                          context,
                          MaterialPageRoute(
                            builder: (context) =>
                                BooksScreen(genre: genre.name),
                          ),
                          (_) => false,
                        ),
                    ),
                    if (!isLast) const TextSpan(text: ", "),
                  ];
                }),
              ],
            ),
          ),
          const SizedBox(height: 16),
          Row(
            children: [
              Expanded(
                child: ElevatedButton(
                  onPressed: () async {
                    if (!AuthProvider.isLoggedIn) {
                      Helpers.showErrorMessage(
                        context,
                        "Please log in to continue".tr(),
                      );
                      return;
                    }
                    if (_book?.publisher?.userId == AuthProvider.userId) {
                      await _openBookFile(_book!.filePath!, _book!.bookId!);
                    } else if (_accessRight == null) {
                      if (_book?.price == 0) {
                        try {
                          await _accessRightsProvider.post(null, _book?.bookId);
                          Helpers.showSuccessMessage(
                            context,
                            "Book is added to your library".tr(),
                          );
                          await _fetchAccessRight();
                        } catch (ex) {
                          Helpers.showErrorMessage(context, ex);
                        }
                      } else {
                        await _openStripePaymentPage();
                      }
                    } else {
                      try {
                        await _accessRightsProvider.delete(_book!.bookId!);
                        Helpers.showSuccessMessage(
                          context,
                          _accessRight?.isHidden == false
                              ? "Book is hidden from your library".tr()
                              : "Book is added to your library".tr(),
                        );
                        await _fetchAccessRight();
                      } catch (ex) {
                        Helpers.showErrorMessage(context, ex);
                      }
                    }
                  },
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Globals.backgroundColor,
                    shape: const RoundedRectangleBorder(
                      borderRadius: BorderRadius.all(Radius.circular(2)),
                    ),
                    padding: const EdgeInsets.symmetric(vertical: 18),
                  ),
                  child: Builder(
                    builder: (context) {
                      final now = DateTime.now().toUtc();
                      final discountActive =
                          _book?.discountPercentage != null &&
                          _book?.discountStart != null &&
                          _book!.discountStart!.isBefore(now) &&
                          _book!.discountEnd!.isAfter(now);
                      final originalPrice = _book?.price ?? 0;
                      final discountedPrice = discountActive
                          ? originalPrice *
                                (1 - (_book!.discountPercentage! / 100))
                          : originalPrice;
                      final bool isHidden = _accessRight?.isHidden == true;
                      if (_book?.price == 0 && _accessRight == null) {
                        return Text(
                          "Add to Library".tr(),
                          style: TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.w700,
                            color: Colors.white,
                          ),
                        );
                      }
                      if (_accessRight != null) {
                        return Text(
                          isHidden
                              ? "Add to Library".tr()
                              : "Hide from Library".tr(),
                          style: const TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.w700,
                            color: Colors.white,
                          ),
                        );
                      }
                      return RichText(
                        text: TextSpan(
                          style: const TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.w700,
                            color: Colors.white,
                          ),
                          children: [
                            if (discountActive) ...[
                              TextSpan(
                                text:
                                    "${discountedPrice.toStringAsFixed(2)}€  ",
                              ),
                              TextSpan(
                                text: "${originalPrice.toStringAsFixed(2)}€",
                                style: TextStyle(
                                  decoration: TextDecoration.lineThrough,
                                  color: Colors.grey[100],
                                ),
                              ),
                            ] else ...[
                              TextSpan(
                                text: "${originalPrice.toStringAsFixed(2)}€",
                              ),
                            ],
                          ],
                        ),
                      );
                    },
                  ),
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: ElevatedButton(
                  onPressed: () => Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) =>
                          PdfScreen(filePath: _book!.filePath!),
                    ),
                  ),
                  style: ElevatedButton.styleFrom(
                    shape: const RoundedRectangleBorder(
                      borderRadius: BorderRadius.all(Radius.circular(2)),
                    ),
                    padding: const EdgeInsets.symmetric(vertical: 18),
                  ),
                  child: Text(
                    "Book Summary".tr(),
                    style: TextStyle(fontSize: 16),
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 20),
          InkWell(
            onTap: () {
              if (_book?.publisher?.userId != null) {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) =>
                        PublisherScreen(publisherId: _book!.publisher!.userId!),
                  ),
                );
              }
            },
            child: Row(
              children: [
                ClipOval(
                  child: Image.network(
                    "${Globals.apiAddress}/images/users/${_book?.publisher?.filePath}.webp?t=${DateTime.now().millisecondsSinceEpoch}",
                    height: 40,
                    width: 40,
                    fit: BoxFit.cover,
                    errorBuilder: (context, error, stackTrace) =>
                        const Icon(Icons.account_circle, size: 40),
                  ),
                ),
                const SizedBox(width: 12),
                Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      _book?.publisher?.userName ?? "",
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                    if (_book?.publisher?.publisherVerifiedById != null)
                      Row(
                        children: [
                          const Icon(
                            Icons.verified,
                            color: Colors.green,
                            size: 16,
                          ),
                          const SizedBox(width: 4),
                          Text(
                            "Verified Publisher".tr(),
                            style: TextStyle(fontSize: 12, color: Colors.green),
                          ),
                        ],
                      ),
                  ],
                ),
              ],
            ),
          ),
          const SizedBox(height: 12),
          Text(
            _book?.description ?? "",
            style: const TextStyle(fontSize: 14, height: 1.4),
            maxLines: 20,
            overflow: TextOverflow.ellipsis,
          ),
          const SizedBox(height: 10),
          if ((_recommendedBooks?.isNotEmpty ?? false)) ...[
            const SizedBox(height: 12),
            Text(
              "You may also like".tr(),
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.w600),
            ),
            const SizedBox(height: 8),
            SizedBox(
              height: 210,
              child: ScrollConfiguration(
                behavior: ScrollConfiguration.of(context).copyWith(
                  dragDevices: {
                    PointerDeviceKind.touch,
                    PointerDeviceKind.mouse,
                  },
                ),
                child: ListView.builder(
                  scrollDirection: Axis.horizontal,
                  itemCount: _recommendedBooks!.length,
                  itemBuilder: (context, index) {
                    final book = _recommendedBooks![index];
                    return Padding(
                      padding: const EdgeInsets.only(right: 8),
                      child: InkWell(
                        onTap: () => Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder: (context) =>
                                BookDetailsScreen(bookId: book.bookId!),
                          ),
                        ),
                        child: Column(
                          children: [
                            ClipRRect(
                              borderRadius: BorderRadius.circular(4),
                              child: Image.network(
                                "${Globals.apiAddress}/images/books/${book.filePath}.webp?t=${DateTime.now().millisecondsSinceEpoch}",
                                height: 165,
                                width: 110,
                                fit: BoxFit.cover,
                                errorBuilder: (context, error, stackTrace) =>
                                    SizedBox(
                                      height: 165,
                                      width: 110,
                                      child: FittedBox(
                                        fit: BoxFit.contain,
                                        child: const Icon(Icons.book),
                                      ),
                                    ),
                              ),
                            ),
                            const SizedBox(height: 4),
                            SizedBox(
                              width: 100,
                              child: Text(
                                book.title ?? "",
                                textAlign: TextAlign.center,
                                maxLines: 2,
                                overflow: TextOverflow.ellipsis,
                                style: const TextStyle(
                                  fontSize: 12,
                                  fontWeight: FontWeight.w500,
                                ),
                              ),
                            ),
                          ],
                        ),
                      ),
                    );
                  },
                ),
              ),
            ),
          ],
          if ((_reviews?.resultList.isNotEmpty ?? false)) ...[
            const SizedBox(height: 10),
            Text(
              "Book reviews".tr(),
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.w600),
            ),
            const SizedBox(height: 2),
            ..._reviews!.resultList.map((review) {
              return Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      ClipOval(
                        child: Image.network(
                          "${Globals.apiAddress}/images/users/${review.user?.filePath}.webp",
                          height: 24,
                          width: 24,
                          fit: BoxFit.cover,
                          errorBuilder: (context, error, stackTrace) =>
                              const Icon(Icons.account_circle, size: 24),
                        ),
                      ),
                      const SizedBox(width: 8),
                      Text(
                        review.user?.userName ?? "",
                        style: const TextStyle(
                          fontWeight: FontWeight.w500,
                          fontSize: 14,
                        ),
                      ),
                      Expanded(
                        child: Row(
                          mainAxisAlignment: MainAxisAlignment.end,
                          children: [
                            Row(
                              children: List.generate(5, (index) {
                                return Icon(
                                  index < (review.rating ?? 0)
                                      ? Icons.star
                                      : Icons.star_border,
                                  size: 15,
                                  color: Colors.orange,
                                );
                              }),
                            ),
                            PopupMenuButton<String>(
                              icon: const Icon(Icons.more_vert, size: 18),
                              onSelected: (value) async {
                                if (value == "edit") {
                                  await _showReviewBookDialog();
                                } else if (value == "delete") {
                                  await _showDeleteReviewDialog();
                                } else if (value == "report") {
                                  if (!AuthProvider.isLoggedIn) {
                                    Helpers.showErrorMessage(
                                      context,
                                      "Please log in to continue".tr(),
                                    );
                                    return;
                                  }
                                  await _showReportReviewDialog(
                                    review.user?.userId ?? 0,
                                  );
                                }
                              },
                              itemBuilder: (context) => [
                                if (review.user?.userId ==
                                    AuthProvider.userId) ...[
                                  PopupMenuItem(
                                    value: "edit",
                                    child: Text("Edit Review").tr(),
                                  ),
                                  PopupMenuItem(
                                    value: "delete",
                                    child: Text("Delete Review").tr(),
                                  ),
                                ],
                                PopupMenuItem(
                                  value: "report",
                                  child: Text("Report Review").tr(),
                                ),
                              ],
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                  if (review.comment?.trim().isNotEmpty == true) ...[
                    Text(
                      review.comment ?? "",
                      style: const TextStyle(fontSize: 14),
                    ),
                    const SizedBox(height: 8),
                  ],
                ],
              );
            }),
          ],
        ],
      ),
    );
  }
}
