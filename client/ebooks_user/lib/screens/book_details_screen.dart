import "package:ebooks_user/models/access_right/access_right.dart";
import "package:ebooks_user/models/books/book.dart";
import "package:ebooks_user/providers/access_rights_provider.dart";
import "package:ebooks_user/providers/auth_provider.dart";
import "package:ebooks_user/providers/books_provider.dart";
import "package:ebooks_user/providers/stripe_provider.dart";
import "package:ebooks_user/screens/books_screen.dart";
import "package:ebooks_user/screens/master_screen.dart";
import "package:ebooks_user/screens/pdf_screen.dart";
import "package:ebooks_user/screens/publisher_screen.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:ebooks_user/utils/helpers.dart";
import "package:flutter/gestures.dart";
import "package:flutter/material.dart";
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
  late StripeProvider _stripeProvider;
  Book? _book;
  final List<String> _allowedActions = [];
  AccessRight? _accessRight;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _booksProvider = context.read<BooksProvider>();
    _accessRightsProvider = context.read<AccessRightsProvider>();
    _stripeProvider = context.read<StripeProvider>();
    _fetchBook();
    _fetchAccessRight();
    _fetchAllowedActions();
  }

  @override
  Widget build(BuildContext context) {
    if (_isLoading) {
      return MasterScreen(
        child: const Center(child: CircularProgressIndicator()),
      );
    }
    return MasterScreen(showBackButton: true, child: _buildResultView());
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

  Future _fetchAllowedActions() async {
    //   try {
    //     final allowedActions = await _booksProvider.getAllowedActions(
    //       widget.bookId,
    //     );
    //     if (!mounted) return;
    //     setState(() {
    //       _allowedActions.clear();
    //       _allowedActions.addAll(allowedActions ?? []);
    //     });
    //   } catch (ex) {
    //     if (!mounted) return;
    //     Helpers.showErrorMessage(context, ex);
    //   } finally {
    //     if (!mounted) return;
    //   }
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

  Widget _buildResultView() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(20),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          _book?.filePath != null
              ? Container(
                  constraints: const BoxConstraints(
                    maxWidth: 400,
                    maxHeight: 400,
                  ),
                  child: Image.network(
                    "${Globals.apiAddress}/images/books/${_book?.filePath}.webp",
                    fit: BoxFit.cover,
                    errorBuilder: (context, error, stackTrace) =>
                        const Icon(Icons.broken_image, size: 400),
                  ),
                )
              : const SizedBox(
                  width: 400,
                  height: 400,
                  child: Icon(Icons.image_not_supported, size: 400),
                ),
          const SizedBox(height: 10),
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                _book?.title ?? "",
                style: const TextStyle(
                  fontSize: 20,
                  fontWeight: FontWeight.w500,
                  height: 1.35,
                ),
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
              ),
              const SizedBox(height: 12),
              Padding(
                padding: const EdgeInsets.only(left: 4),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    GestureDetector(
                      onTap: () {
                        Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder: (context) => PublisherScreen(
                              publisherId: _book!.publisher!.userId!,
                            ),
                          ),
                        );
                      },
                      child: Row(
                        children: [
                          ClipOval(
                            child: Image.network(
                              "${Globals.apiAddress}/images/users/${_book?.publisher?.filePath}.webp",
                              height: 18,
                              width: 18,
                              fit: BoxFit.cover,
                              errorBuilder: (context, error, stackTrace) =>
                                  const Icon(Icons.account_circle, size: 18),
                            ),
                          ),
                          const SizedBox(width: 6),
                          Text(
                            _book?.publisher?.userName ?? "",
                            style: const TextStyle(
                              fontSize: 13,
                              fontWeight: FontWeight.w600,
                            ),
                            overflow: TextOverflow.ellipsis,
                          ),
                          const SizedBox(width: 4),
                          if (_book?.publisher?.publisherVerifiedById != null)
                            const Padding(
                              padding: EdgeInsets.only(top: 2),
                              child: Icon(
                                Icons.verified,
                                color: Colors.green,
                                size: 13,
                              ),
                            ),
                        ],
                      ),
                    ),
                    const SizedBox(height: 12),
                    RichText(
                      text: TextSpan(
                        style: TextStyle(
                          color: Theme.of(context).textTheme.bodyMedium?.color,
                        ),
                        children: [
                          TextSpan(
                            text: (_book?.authors?.length ?? 1) == 1
                                ? "Author: "
                                : "Authors: ",
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
                                ),
                                recognizer: TapGestureRecognizer()
                                  ..onTap = () {
                                    Navigator.pop(context);
                                    Navigator.pushAndRemoveUntil(
                                      context,
                                      MaterialPageRoute(
                                        builder: (context) =>
                                            BooksScreen(author: author.name),
                                      ),
                                      (_) => false,
                                    );
                                  },
                              ),
                              if (!isLast)
                                TextSpan(
                                  text: ", ",
                                  style: TextStyle(
                                    color: Theme.of(
                                      context,
                                    ).textTheme.bodyMedium?.color,
                                  ),
                                ),
                            ];
                          }),
                        ],
                      ),
                    ),
                    RichText(
                      text: TextSpan(
                        style: TextStyle(
                          color: Theme.of(context).textTheme.bodyMedium?.color,
                        ),
                        children: [
                          TextSpan(
                            text: (_book?.genres?.length ?? 1) == 1
                                ? "Genre: "
                                : "Genres: ",
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
                                ),
                                recognizer: TapGestureRecognizer()
                                  ..onTap = () {
                                    Navigator.pop(context);
                                    Navigator.pushAndRemoveUntil(
                                      context,
                                      MaterialPageRoute(
                                        builder: (context) =>
                                            BooksScreen(genre: genre.name),
                                      ),
                                      (_) => false,
                                    );
                                  },
                              ),
                              if (!isLast)
                                TextSpan(
                                  text: ", ",
                                  style: TextStyle(
                                    color: Theme.of(
                                      context,
                                    ).textTheme.bodyMedium?.color,
                                  ),
                                ),
                            ];
                          }),
                        ],
                      ),
                    ),
                    RichText(
                      text: TextSpan(
                        children: [
                          TextSpan(
                            text: "Language: ",
                            style: TextStyle(
                              color: Theme.of(
                                context,
                              ).textTheme.bodyMedium?.color,
                            ),
                          ),
                          TextSpan(
                            text: _book?.language?.name ?? "",
                            style: TextStyle(color: Globals.backgroundColor),
                            recognizer: TapGestureRecognizer()
                              ..onTap = () {
                                Navigator.pop(context);
                                Navigator.pushAndRemoveUntil(
                                  context,
                                  MaterialPageRoute(
                                    builder: (context) => BooksScreen(
                                      language: _book?.language?.name,
                                    ),
                                  ),
                                  (_) => false,
                                );
                              },
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(height: 2),
                    RichText(
                      text: TextSpan(
                        children: [
                          TextSpan(
                            text: "Number of pages: ",
                            style: TextStyle(
                              color: Theme.of(
                                context,
                              ).textTheme.bodyMedium?.color,
                            ),
                          ),
                          TextSpan(
                            text: _book?.numberOfPages.toString(),
                            style: TextStyle(
                              color: Theme.of(
                                context,
                              ).textTheme.bodyMedium?.color,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 18),
              Column(
                children: [
                  SizedBox(
                    width: double.infinity,
                    child: ElevatedButton(
                      onPressed: () async {
                        if (!AuthProvider.isLoggedIn) {
                          Helpers.showErrorMessage(
                            context,
                            "Please log in to continue",
                          );
                          return;
                        }
                        if (_accessRight == null) {
                          if (_book?.price == 0) {
                            try {
                              await _accessRightsProvider.post(
                                null,
                                _book?.bookId,
                              );
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
                          borderRadius: BorderRadius.all(Radius.circular(10)),
                        ),
                        padding: const EdgeInsets.symmetric(vertical: 16),
                      ),
                      child: Text(
                        _accessRight == null
                            ? (_book?.price == 0
                                  ? "Add to Library"
                                  : "Buy ${_book?.price?.toStringAsFixed(2)}€")
                            : (_accessRight?.isHidden == false
                                  ? "Hide from Library"
                                  : "Add to Library"),
                        style: const TextStyle(
                          fontSize: 16,
                          color: Colors.white,
                        ),
                      ),
                    ),
                  ),
                  const SizedBox(height: 6),
                  SizedBox(
                    width: double.infinity,
                    child: OutlinedButton(
                      onPressed: () {
                        Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder: (context) =>
                                PdfScreen(filePath: _book?.filePath),
                          ),
                        );
                      },
                      style: OutlinedButton.styleFrom(
                        side: const BorderSide(
                          color: Globals.backgroundColor,
                          width: 2,
                        ),
                        shape: const RoundedRectangleBorder(
                          borderRadius: BorderRadius.all(Radius.circular(10)),
                        ),
                        padding: const EdgeInsets.symmetric(vertical: 16),
                      ),
                      child: const Text(
                        "Summary of the Book",
                        style: TextStyle(
                          fontSize: 16,
                          color: Globals.backgroundColor,
                        ),
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 24),
              Padding(
                padding: const EdgeInsets.only(left: 4),
                child: Text(_book?.description ?? ""),
              ),
            ],
          ),
        ],
      ),
    );
  }
}
