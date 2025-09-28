import "package:ebooks_user/models/books/book.dart";
import "package:ebooks_user/models/search_result.dart";
import "package:ebooks_user/providers/auth_provider.dart";
import "package:ebooks_user/providers/books_provider.dart";
import "package:ebooks_user/providers/publisher_follows_provider.dart";
import "package:ebooks_user/providers/reports_provider.dart";
import "package:ebooks_user/providers/wishlist_provider.dart";
import "package:ebooks_user/screens/master_screen.dart";
import "package:ebooks_user/utils/helpers.dart";
import "package:ebooks_user/widgets/book_card_view.dart";
import "package:flutter/material.dart";
import "package:flutter/services.dart";
import "package:provider/provider.dart";
import 'package:easy_localization/easy_localization.dart';

class BooksScreen extends StatefulWidget {
  final String? author;
  final String? genre;
  final String? language;
  const BooksScreen({super.key, this.author, this.genre, this.language});

  @override
  State<BooksScreen> createState() => _BooksScreenState();
}

class _BooksScreenState extends State<BooksScreen> {
  late BooksProvider _booksProvider;
  late WishlistProvider _wishlistProvider;
  late PublisherFollowsProvider _publisherFollowsProvider;
  late ReportsProvider _reportsProvider;
  SearchResult<Book>? _books;
  bool _isLoading = true;
  int _currentPage = 1;
  Map<String, dynamic> _currentFilter = {};
  final TextEditingController _searchController = TextEditingController();
  final ScrollController _scrollController = ScrollController();
  String author = "";
  String genre = "";
  String language = "";
  int? minPrice;
  int? maxPrice;
  String orderBy = "Last added";
  late TextEditingController _authorController;
  late TextEditingController _genreController;
  late TextEditingController _languageController;
  late TextEditingController _minPriceController;
  late TextEditingController _maxPriceController;

  @override
  void initState() {
    super.initState();
    _booksProvider = context.read<BooksProvider>();
    _wishlistProvider = context.read<WishlistProvider>();
    _publisherFollowsProvider = context.read<PublisherFollowsProvider>();
    _reportsProvider = context.read<ReportsProvider>();
    _currentFilter = {
      "Status": "Approved",
      "IsDeleted": "Not deleted",
      "IsReviewsIncluded": true,
      "Author": widget.author,
      "Genre": widget.genre,
      "Language": widget.language,
    };
    _fetchBooks();
    _authorController = TextEditingController(text: author);
    _genreController = TextEditingController(text: genre);
    _languageController = TextEditingController(text: language);
    _minPriceController = TextEditingController(
      text: minPrice?.toString() ?? "",
    );
    _maxPriceController = TextEditingController(
      text: maxPrice?.toString() ?? "",
    );
    _scrollController.addListener(() {
      if (_scrollController.position.pixels >=
          _scrollController.position.maxScrollExtent - 200) {
        if (!_isLoading &&
            (_books?.resultList.length ?? 0) < (_books?.count ?? 0)) {
          _currentPage++;
          _fetchBooks(append: true);
        }
      }
    });
  }

  @override
  void dispose() {
    _scrollController.dispose();
    _searchController.dispose();
    _authorController.dispose();
    _genreController.dispose();
    _languageController.dispose();
    _minPriceController.dispose();
    _maxPriceController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      searchController: _searchController,
      onSearch: () async => await _applySearchFilter(),
      onFilterPressed: () async => await _showFilterDialog(),
      child: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _buildResultView(),
    );
  }

  Future _fetchBooks({bool append = false}) async {
    setState(() => _isLoading = true);
    try {
      final books = await _booksProvider.getPaged(
        page: _currentPage,
        filter: _currentFilter,
      );
      if (!mounted) return;
      setState(() {
        if (append && _books != null) {
          _books?.resultList.addAll(books.resultList);
          _books?.count = books.count;
        } else {
          _books = books;
        }
      });
    } catch (ex) {
      if (!mounted) return;
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (!mounted) return;
        Helpers.showErrorMessage(context, ex);
      });
    } finally {
      if (!mounted) return;
      setState(() => _isLoading = false);
    }
  }

  Future _applySearchFilter() async {
    _currentPage = 1;
    _currentFilter = {
      "Status": "Approved",
      "IsDeleted": "Not deleted",
      if (_searchController.text.isNotEmpty) "Title": _searchController.text,
    };
    await _fetchBooks();
  }

  Future _showFilterDialog() async {
    await showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: Text("Filter books".tr()),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextField(
                controller: _authorController,
                decoration: InputDecoration(labelText: "Author".tr()),
              ),
              TextField(
                controller: _genreController,
                decoration: InputDecoration(labelText: "Genre".tr()),
              ),
              TextField(
                controller: _languageController,
                decoration: InputDecoration(labelText: "Language".tr()),
              ),
              Row(
                children: [
                  Expanded(
                    child: TextField(
                      controller: _minPriceController,
                      keyboardType: TextInputType.number,
                      inputFormatters: [FilteringTextInputFormatter.digitsOnly],
                      decoration: InputDecoration(
                        labelText: "Min price (€)".tr(),
                      ),
                    ),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: TextField(
                      controller: _maxPriceController,
                      keyboardType: TextInputType.number,
                      inputFormatters: [FilteringTextInputFormatter.digitsOnly],
                      decoration: InputDecoration(
                        labelText: "Max price (€)".tr(),
                      ),
                      onChanged: (value) {
                        maxPrice = int.tryParse(value);
                      },
                    ),
                  ),
                ],
              ),
              DropdownButtonFormField<String>(
                decoration: InputDecoration(labelText: "Sort by".tr()),
                value: orderBy,
                items:
                    [
                      {"value": "Last added", "label": "Last added".tr()},
                      {"value": "Most views", "label": "Most views".tr()},
                      {"value": "Highest rated", "label": "Highest rated".tr()},
                      {"value": "Lowest price", "label": "Lowest price".tr()},
                      {"value": "Highest price", "label": "Highest price".tr()},
                    ].map((item) {
                      return DropdownMenuItem<String>(
                        value: item["value"],
                        child: Text(item["label"]!.tr()),
                      );
                    }).toList(),
                onChanged: (value) {
                  setState(() {
                    orderBy = value!;
                  });
                },
              ),
            ],
          ),
          actions: [
            TextButton(
              onPressed: () {
                Navigator.pop(context);
                _currentPage = 1;
                _currentFilter = {
                  "Title": _searchController.text,
                  "Author": _authorController.text,
                  "Genre": _genreController.text,
                  "Language": _languageController.text,
                  "MinPrice": _minPriceController.text,
                  "MaxPrice": _maxPriceController.text,
                  "OrderBy": orderBy,
                  "Status": "Approved",
                  "IsDeleted": "Not deleted",
                };
                _fetchBooks();
              },
              child: Text("Apply".tr()),
            ),
            TextButton(
              onPressed: () => Navigator.pop(context),
              child: Text("Cancel".tr()),
            ),
          ],
        );
      },
    );
  }

  Future _showReportBookDialog(int bookId) async {
    String reason = "";
    await showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: Text("Confirm report".tr()),
          content: TextField(
            decoration: InputDecoration(labelText: "Reason...".tr()),
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
              child: Text("Report".tr()),
            ),
            TextButton(
              onPressed: () => Navigator.pop(context),
              child: Text("Cancel".tr()),
            ),
          ],
        );
      },
    );
  }

  Widget _buildResultView() {
    final books = _books?.resultList ?? [];
    return ListView.builder(
      controller: _scrollController,
      padding: const EdgeInsets.symmetric(vertical: 8, horizontal: 6),
      itemCount: books.length + (_isLoading ? 1 : 0),
      itemBuilder: (context, index) {
        if (_isLoading && index == books.length) {
          return const Padding(
            padding: EdgeInsets.all(16),
            child: Center(child: CircularProgressIndicator()),
          );
        }
        return BookCardView(
          book: books[index],
          popupActions: {
            "Add to wishlist".tr(): () async {
              try {
                await _wishlistProvider.post(null, books[index].bookId);
                Helpers.showSuccessMessage(
                  context,
                  "Book is added to your wishlist".tr(),
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
            },
            "Follow publisher".tr(): () async {
              try {
                await _publisherFollowsProvider.post(
                  null,
                  books[index].publisher?.userId,
                );
                Helpers.showSuccessMessage(
                  context,
                  "You are now following ${books[index].publisher?.userName}"
                      .tr(),
                );
              } catch (ex) {
                AuthProvider.isLoggedIn
                    ? Helpers.showErrorMessage(
                        context,
                        "You already follow ${books[index].publisher?.userName}"
                            .tr(),
                      )
                    : Helpers.showErrorMessage(
                        context,
                        "You must be logged in".tr(),
                      );
              }
            },
            "Report book".tr(): () async {
              AuthProvider.isLoggedIn
                  ? await _showReportBookDialog(books[index].bookId!)
                  : Helpers.showErrorMessage(
                      context,
                      "You must be logged in".tr(),
                    );
            },
          },
        );
      },
    );
  }
}
