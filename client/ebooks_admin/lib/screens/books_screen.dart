import "package:ebooks_admin/models/books/book.dart";
import "package:ebooks_admin/models/search_result.dart";
import "package:ebooks_admin/providers/books_provider.dart";
import "package:ebooks_admin/screens/book_details_screen.dart";
import "package:ebooks_admin/screens/master_screen.dart";
import "package:ebooks_admin/utils/globals.dart";
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
  final TextEditingController _titleController = TextEditingController();
  final TextEditingController _publisherController = TextEditingController();
  final TextEditingController _languageController = TextEditingController();

  @override
  void initState() {
    super.initState();
    _currentFilter = {"Status": widget.status ?? ""};
    _booksProvider = context.read<BooksProvider>();
    _fetchBooks();
    _fetchBookStates();
  }

  @override
  void dispose() {
    _titleController.dispose();
    _publisherController.dispose();
    _languageController.dispose();
    super.dispose();
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
    try {
      final bookStates = await _booksProvider.getBookStates();
      if (!mounted) return;
      setState(() => _bookStates.addAll(bookStates ?? []));
      _status = widget.status ?? "Any";
    } catch (ex) {
      if (!mounted) return;
      Helpers.showErrorMessage(context, ex);
    } finally {
      if (!mounted) return;
    }
  }

  Widget _buildSearch() {
    return Padding(
      padding: const EdgeInsets.all(Globals.spacing),
      child: Row(
        children: [
          Expanded(
            child: TextField(
              controller: _titleController,
              decoration: const InputDecoration(labelText: "Title"),
            ),
          ),
          const SizedBox(width: Globals.spacing),
          Expanded(
            child: TextField(
              controller: _publisherController,
              decoration: const InputDecoration(labelText: "Publisher"),
            ),
          ),
          const SizedBox(width: Globals.spacing),
          Expanded(
            child: TextField(
              controller: _languageController,
              decoration: const InputDecoration(labelText: "Language"),
            ),
          ),
          const SizedBox(width: Globals.spacing),
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
          const SizedBox(width: Globals.spacing),
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
            ),
          ),
          const SizedBox(width: Globals.spacing),
          Expanded(
            child: DropdownButtonFormField<String>(
              value: _orderBy,
              onChanged: (value) {
                _orderBy = value!;
              },
              items: ["Last modified", "First modified", "Title", "Publisher"]
                  .map((String value) {
                    return DropdownMenuItem<String>(
                      value: value,
                      child: Text(
                        value,
                        style: const TextStyle(fontWeight: FontWeight.normal),
                      ),
                    );
                  })
                  .toList(),
              decoration: const InputDecoration(labelText: "Sort by"),
            ),
          ),
          const SizedBox(width: Globals.spacing),
          ElevatedButton(
            onPressed: () async {
              _currentPage = 1;
              _currentFilter = {
                "Title": _titleController.text,
                "Publisher": _publisherController.text,
                "Language": _languageController.text,
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
                                onPressed: () => Navigator.push(
                                  context,
                                  MaterialPageRoute(
                                    builder: (context) =>
                                        BookDetailsScreen(bookId: book.bookId!),
                                  ),
                                ),
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
      padding: const EdgeInsets.symmetric(vertical: Globals.spacing),
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
