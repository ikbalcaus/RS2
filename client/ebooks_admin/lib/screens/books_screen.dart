import "package:ebooks_admin/models/books/book.dart";
import "package:ebooks_admin/models/search_result.dart";
import "package:ebooks_admin/providers/books_provider.dart";
import "package:ebooks_admin/screens/book_details_screen.dart";
import "package:ebooks_admin/screens/master_screen.dart";
import "package:ebooks_admin/utils/constants.dart";
import "package:ebooks_admin/utils/helpers.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";

class BooksScreen extends StatefulWidget {
  final String? status;
  const BooksScreen({super.key, this.status});

  @override
  State<BooksScreen> createState() => _BooksScreenState();
}

class _BooksScreenState extends State<BooksScreen> {
  late BooksProvider _booksProvider;
  SearchResult<Book>? _books;
  bool _isLoading = true;
  int _currentPage = 1;
  String _status = "Any";
  String _isDeleted = "All books";
  String _orderBy = "Last modified";
  final List<String> _bookStates = ["Any"];
  Map<String, dynamic> _currentFilter = {};

  final TextEditingController _titleEditingController = TextEditingController();
  final TextEditingController _publisherEditingController =
      TextEditingController();
  final TextEditingController _languageEditingController =
      TextEditingController();

  @override
  void initState() {
    super.initState();
    _currentFilter = {"Status": widget.status ?? ""};
    _booksProvider = context.read<BooksProvider>();
    _fetchBooks();
    _fetchBookStates();
  }

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _buildSearch(),
          Expanded(
            child: _isLoading
                ? const Center(child: CircularProgressIndicator())
                : _buildResultView(),
          ),
          _buildPagination(),
        ],
      ),
    );
  }

  Future _fetchBooks() async {
    setState(() => _isLoading = true);
    try {
      final books = await _booksProvider.getPaged(
        page: _currentPage,
        filter: _currentFilter,
      );
      if (!mounted) return;
      setState(() => _books = books);
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

  Future _fetchBookStates() async {
    final bookStates = await _booksProvider.getBookStates();
    if (!mounted) return;
    setState(() => _bookStates.addAll(bookStates ?? []));
    _status = widget.status ?? "Any";
  }

  Widget _buildSearch() {
    return Padding(
      padding: const EdgeInsets.all(Constants.defaultSpacing),
      child: Row(
        children: [
          Expanded(
            child: TextField(
              controller: _titleEditingController,
              decoration: const InputDecoration(labelText: "Title"),
            ),
          ),
          const SizedBox(width: Constants.defaultSpacing),
          Expanded(
            child: TextField(
              controller: _publisherEditingController,
              decoration: const InputDecoration(labelText: "Publisher"),
            ),
          ),
          const SizedBox(width: Constants.defaultSpacing),
          Expanded(
            child: TextField(
              controller: _languageEditingController,
              decoration: const InputDecoration(labelText: "Language"),
            ),
          ),
          const SizedBox(width: Constants.defaultSpacing),
          Expanded(
            child: DropdownButtonFormField<String>(
              value: _status,
              onChanged: (value) {
                _status = value!;
              },
              items: _bookStates.map((String value) {
                return DropdownMenuItem<String>(
                  value: value,
                  child: Text(
                    value,
                    style: const TextStyle(fontWeight: FontWeight.normal),
                  ),
                );
              }).toList(),
              decoration: const InputDecoration(labelText: "Status"),
            ),
          ),
          const SizedBox(width: Constants.defaultSpacing),
          Expanded(
            child: DropdownButtonFormField<String>(
              value: _isDeleted,
              onChanged: (value) {
                _isDeleted = value!;
              },
              items: ["All books", "Not deleted", "Deleted"].map((
                String value,
              ) {
                return DropdownMenuItem<String>(
                  value: value,
                  child: Text(
                    value,
                    style: const TextStyle(fontWeight: FontWeight.normal),
                  ),
                );
              }).toList(),
              decoration: const InputDecoration(labelText: "Is deleted"),
            ),
          ),
          const SizedBox(width: Constants.defaultSpacing),
          Expanded(
            child: DropdownButtonFormField<String>(
              value: _orderBy,
              onChanged: (value) {
                _orderBy = value!;
              },
              items:
                  [
                    "Last modified",
                    "First modified",
                    "Title (A-Z)",
                    "Title (Z-A)",
                    "Publisher (A-Z)",
                    "Publisher (Z-A)",
                  ].map((String value) {
                    return DropdownMenuItem<String>(
                      value: value,
                      child: Text(
                        value,
                        style: const TextStyle(fontWeight: FontWeight.normal),
                      ),
                    );
                  }).toList(),
              decoration: const InputDecoration(labelText: "Sort by"),
            ),
          ),
          const SizedBox(width: Constants.defaultSpacing),
          ElevatedButton(
            onPressed: () async {
              _currentPage = 1;
              _currentFilter = {
                "Title": _titleEditingController.text,
                "Publisher": _publisherEditingController.text,
                "Language": _languageEditingController.text,
                "Status": _status,
                "IsDeleted": _isDeleted,
                "OrderBy": _orderBy,
              };
              await _fetchBooks();
            },
            child: const Text("Search"),
          ),
        ],
      ),
    );
  }

  Widget _buildResultView() {
    return SizedBox(
      width: double.infinity,
      child: SingleChildScrollView(
        child: DataTable(
          columns: const [
            DataColumn(label: Text("Title")),
            DataColumn(label: Text("Price")),
            DataColumn(label: Text("Publisher")),
            DataColumn(label: Text("Language")),
            DataColumn(label: Text("Status")),
            DataColumn(label: Text("Actions")),
          ],
          rows:
              _books?.resultList
                  .map(
                    (book) => DataRow(
                      cells: [
                        DataCell(Text(book.title ?? "")),
                        DataCell(Text("${book.price?.toString()}€")),
                        DataCell(Text(book.publisher?.userName ?? "")),
                        DataCell(Text(book.language?.name ?? "")),
                        DataCell(Text(book.status ?? "")),
                        DataCell(
                          Row(
                            children: [
                              IconButton(
                                icon: const Icon(Icons.arrow_forward),
                                tooltip: "Open details",
                                onPressed: () {
                                  Navigator.push(
                                    context,
                                    MaterialPageRoute(
                                      builder: (context) => BookDetailsScreen(
                                        bookId: book.bookId!,
                                      ),
                                    ),
                                  );
                                },
                              ),
                            ],
                          ),
                        ),
                      ],
                    ),
                  )
                  .toList() ??
              [],
        ),
      ),
    );
  }

  Widget _buildPagination() {
    if (_books == null || _books!.totalPages <= 1) {
      return const SizedBox.shrink();
    }
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: Constants.defaultSpacing),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          IconButton(
            icon: const Icon(Icons.chevron_left),
            onPressed: _currentPage > 1
                ? () async {
                    _isLoading = true;
                    _currentPage -= 1;
                    await _fetchBooks();
                  }
                : null,
          ),
          Text("Page $_currentPage of ${_books!.totalPages}"),
          IconButton(
            icon: const Icon(Icons.chevron_right),
            onPressed: _currentPage < _books!.totalPages
                ? () async {
                    _isLoading = true;
                    _currentPage += 1;
                    await _fetchBooks();
                  }
                : null,
          ),
        ],
      ),
    );
  }
}
