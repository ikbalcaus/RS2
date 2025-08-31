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
        _popupActions["Add to Wishlist"] = () async {
          try {
            await _wishlistProvider.post(null, _book?.bookId);
            Helpers.showSuccessMessage(context, "Book is added to wishlist");
          } catch (ex) {
            AuthProvider.isLoggedIn
                ? Helpers.showErrorMessage(
                    context,
                    "Book is already in your wishlist",
                  )
                : Helpers.showErrorMessage(context, "You must be logged in");
          }
        };
        _popupActions["Follow Publisher"] = () async {
          try {
            await _publisherFollowsProvider.post(
              null,
              _book?.publisher?.userId,
            );
            Helpers.showSuccessMessage(
              context,
              "You are now following ${_book?.publisher?.userName}",
            );
          } catch (ex) {
            AuthProvider.isLoggedIn
                ? Helpers.showErrorMessage(
                    context,
                    "You already follow ${_book?.publisher?.userName}",
                  )
                : Helpers.showErrorMessage(context, "You must be logged in");
          }
        };
        if (_accessRight != null &&
            _book?.publisher?.userId != AuthProvider.userId) {
          _popupActions["Review Book"] = () async =>
              await _showReviewBookDialog();
        }
        if (_allowedActions.contains("Update")) {
          _popupActions["Edit Book"] = () async => Navigator.push(
            context,
            MaterialPageRoute(
              builder: (context) => EditBookScreen(bookId: _book!.bookId!),
            ),
          );
        }
        if (_allowedActions.contains("Await")) {
          _popupActions["Publish Book"] = () async =>
              await _showAwaitBookDialog();
        }
        if (_allowedActions.contains("Hide")) {
          _popupActions[_book?.status == "Approved"
              ? "Hide Book"
              : "Unhide Book"] = () async =>
              await _showHideBookDialog();
        }
        _popupActions["Report Book"] = () async {
          AuthProvider.isLoggedIn
              ? await _showReportBookDialog(widget.bookId)
              : Helpers.showErrorMessage(context, "You must be logged in");
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
        Helpers.showSuccessMessage(context, "Book is downloading...");
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
          "Cannot opet an URL: ${uri.toString()}",
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
              title: const Text("Confirm review"),
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
                    decoration: const InputDecoration(labelText: "Comment..."),
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
                            "Book reviewed successfully",
                          );
                        }
                      } catch (ex) {
                        Navigator.pop(context);
                        Helpers.showErrorMessage(context, ex);
                      }
                    }
                  },
                  child: const Text("Submit"),
                ),
                TextButton(
                  onPressed: () => Navigator.pop(context),
                  child: const Text("Cancel"),
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
        title: const Text("Delete review"),
        content: const Text("Are you sure you want to delete review?"),
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
                    "Successfully deleted review",
                  );
                }
              } catch (ex) {
                Navigator.pop(context);
                Helpers.showErrorMessage(context, ex);
              }
            },
            child: const Text("Delete"),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text("Cancel"),
          ),
        ],
      ),
    );
  }

  Future _showReportBookDialog(int bookId) async {
    String reason = "";
    await showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: const Text("Confirm report"),
          content: TextField(
            decoration: const InputDecoration(labelText: "Reason..."),
            onChanged: (value) => reason = value,
          ),
          actions: [
            TextButton(
              onPressed: () async {
                if (reason.trim().isNotEmpty) {
                  try {
                    await _reportsProvider.post({"reason": reason}, bookId);
                    if (context.mounted) {
                      Navigator.pop(context);
                      Helpers.showSuccessMessage(
                        context,
                        "Successfully reported book",
                      );
                    }
                  } catch (ex) {
                    Navigator.pop(context);
                    Helpers.showErrorMessage(
                      context,
                      "You already reported this book",
                    );
                  }
                }
              },
              child: const Text("Report"),
            ),
            TextButton(
              onPressed: () => Navigator.pop(context),
              child: const Text("Cancel"),
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
          title: const Text("Confirm report"),
          content: TextField(
            decoration: const InputDecoration(labelText: "Reason..."),
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
                        "Successfully reported review",
                      );
                    }
                  } catch (ex) {
                    Navigator.pop(context);
                    Helpers.showErrorMessage(context, ex);
                  }
                }
              },
              child: const Text("Report"),
            ),
            TextButton(
              onPressed: () => Navigator.pop(context),
              child: const Text("Cancel"),
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
        title: const Text("Publish book"),
        content: const Text("Are you sure you want to await this book?"),
        actions: [
          TextButton(
            onPressed: () async {
              try {
                await _booksProvider.awaitBook(_book!.bookId!);
                Helpers.showSuccessMessage(
                  context,
                  "Book is now pending review by moderators",
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
            child: const Text("Publish"),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text("Cancel"),
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
            ? const Text("Hide book")
            : const Text("Unhide book"),
        content: const Text("Are you sure you want to hide this book?"),
        actions: [
          TextButton(
            onPressed: () async {
              try {
                await _booksProvider.hideBook(_book!.bookId!);
                Helpers.showSuccessMessage(
                  context,
                  _book?.status == "Approved"
                      ? "Book is now hidden to other users"
                      : "Book is now available to other users",
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
                ? const Text("Hide")
                : const Text("Unhide"),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text("Cancel"),
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
                    const Text("Views", style: TextStyle(fontSize: 14)),
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
                    Text("Rating", style: TextStyle(fontSize: 14)),
                    SizedBox(height: 4),
                    Text(
                      (_book?.averageRating == 0.0)
                          ? "N/A"
                          : (_book?.averageRating ?? "").toString(),
                      style: TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                  ],
                ),
                Column(
                  children: [
                    const Text("Language", style: TextStyle(fontSize: 14)),
                    const SizedBox(height: 4),
                    Text(
                      _book?.language?.name ?? "N/A",
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                  ],
                ),
                Column(
                  children: [
                    const Text("Pages", style: TextStyle(fontSize: 14)),
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
                      ? "Author: "
                      : "Authors: ",
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
                      ? "Genre: "
                      : "Genres: ",
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
                        "Please log in to continue",
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
                            "Book is added to your library",
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
                              ? "Book is hidden from your library"
                              : "Book is added to your library",
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
                          _book?.discountEnd != null &&
                          _book!.discountStart!.isBefore(now) &&
                          _book!.discountEnd!.isAfter(now);
                      final originalPrice = _book?.price ?? 0;
                      final discountedPrice = discountActive
                          ? originalPrice *
                                (1 - (_book!.discountPercentage! / 100))
                          : originalPrice;
                      final bool isHidden = _accessRight?.isHidden == true;
                      if (_book?.price == 0 && _accessRight == null) {
                        return const Text(
                          "Add to Library",
                          style: TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.w700,
                            color: Colors.white,
                          ),
                        );
                      }
                      if (_accessRight != null) {
                        return Text(
                          isHidden ? "Add to Library" : "Hide from Library",
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
                                  color: Colors.grey[400],
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
                  child: const Text(
                    "Book Summary",
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
                        children: const [
                          Icon(Icons.verified, color: Colors.green, size: 16),
                          SizedBox(width: 4),
                          Text(
                            "Verified Publisher",
                            style: TextStyle(fontSize: 12, color: Colors.green),
                          ),
                        ],
                      ),
                  ],
                ),
              ],
            ),
          ),
          const SizedBox(height: 20),
          Text(
            _book?.description ?? "",
            style: const TextStyle(fontSize: 14, height: 1.4),
            maxLines: 20,
            overflow: TextOverflow.ellipsis,
          ),
          const SizedBox(height: 12),
          ...(_reviews?.resultList ?? []).map((review) {
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
                                    "Please log in to continue",
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
                                const PopupMenuItem(
                                  value: "edit",
                                  child: Text("Edit Review"),
                                ),
                                const PopupMenuItem(
                                  value: "delete",
                                  child: Text("Delete Review"),
                                ),
                              ],
                              const PopupMenuItem(
                                value: "report",
                                child: Text("Report Review"),
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
                  const SizedBox(height: 10),
                ],
              ],
            );
          }),
        ],
      ),
    );
  }
}
