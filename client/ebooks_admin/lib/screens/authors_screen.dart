import "package:ebooks_admin/models/authors/author.dart";
import "package:ebooks_admin/models/search_result.dart";
import "package:ebooks_admin/providers/authors_provider.dart";
import "package:ebooks_admin/screens/master_screen.dart";
import "package:ebooks_admin/utils/globals.dart";
import "package:ebooks_admin/utils/helpers.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";

class AuthorsScreen extends StatefulWidget {
  final String? name;
  const AuthorsScreen({super.key, this.name});

  @override
  State<AuthorsScreen> createState() => _AuthorsScreenState();
}

class _AuthorsScreenState extends State<AuthorsScreen> {
  late AuthorsProvider _authorsProvider;
  SearchResult<Author>? _authors;
  bool _isLoading = true;
  int _currentPage = 1;
  String _orderBy = "Last modified";
  Map<String, dynamic> _currentFilter = {};
  final TextEditingController _nameController = TextEditingController();

  @override
  void initState() {
    super.initState();
    _nameController.text = widget.name ?? "";
    _currentFilter = {"name": widget.name ?? ""};
    _authorsProvider = context.read<AuthorsProvider>();
    _fetchAuthors();
  }

  @override
  void dispose() {
    _nameController.dispose();
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

  Future _fetchAuthors() async {
    setState(() => _isLoading = true);
    try {
      final authors = await _authorsProvider.getPaged(
        page: _currentPage,
        filter: _currentFilter,
      );
      if (!mounted) return;
      setState(() => _authors = authors);
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

  Future _showEditAuthorDialog(int id, String name) async {
    String name = _nameController.text;
    await showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: const Text("Confirm edit"),
          content: TextField(
            decoration: const InputDecoration(labelText: "Enter new name..."),
            onChanged: (value) => name = value,
          ),
          actions: [
            TextButton(
              onPressed: () async {
                if (name.trim().isNotEmpty) {
                  try {
                    await _authorsProvider.put(id, {"name": name});
                    await _fetchAuthors();
                    if (context.mounted) {
                      Navigator.pop(context);
                      Helpers.showSuccessMessage(context);
                    }
                  } catch (ex) {
                    Helpers.showErrorMessage(context, ex);
                  }
                }
              },
              child: const Text("Edit"),
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

  Future _showDeleteAuthorDialog(int id) async {
    await showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: const Text("Confirm delete"),
          actions: [
            TextButton(
              onPressed: () async {
                Navigator.pop(context);
                await Future.delayed(const Duration(milliseconds: 250));
                try {
                  await _authorsProvider.delete(id);
                  Helpers.showSuccessMessage(context);
                  await _fetchAuthors();
                } catch (ex) {
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
        );
      },
    );
  }

  Widget _buildSearch() {
    return Padding(
      padding: const EdgeInsets.all(Globals.spacing),
      child: Row(
        children: [
          Expanded(
            child: TextField(
              controller: _nameController,
              decoration: const InputDecoration(labelText: "Author"),
            ),
          ),
          const SizedBox(width: Globals.spacing),
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
                    "Name (A-Z)",
                    "Name (Z-A)",
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
          const SizedBox(width: Globals.spacing),
          ElevatedButton(
            onPressed: () async {
              _currentPage = 1;
              _currentFilter = {
                "Name": _nameController.text,
                "OrderBy": _orderBy,
              };
              await _fetchAuthors();
            },
            child: const Text("Search"),
          ),
          const SizedBox(width: Globals.spacing),
          ElevatedButton(
            onPressed: () async {
              try {
                await _authorsProvider.post({"name": _nameController.text});
                await _fetchAuthors();
                Helpers.showSuccessMessage(context);
                _nameController.clear();
              } catch (ex) {
                Helpers.showErrorMessage(context, ex);
              }
            },
            child: const Text("Add new"),
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
            DataColumn(label: Text("Author")),
            DataColumn(label: Text("Modified by")),
            DataColumn(label: Text("Actions")),
          ],
          rows:
              _authors?.resultList
                  .map(
                    (author) => DataRow(
                      cells: [
                        DataCell(Text(author.name ?? "")),
                        DataCell(Text(author.modifiedBy?.userName ?? "")),
                        DataCell(
                          Row(
                            children: [
                              IconButton(
                                icon: const Icon(Icons.edit),
                                tooltip: "Edit author",
                                onPressed: () async {
                                  await _showEditAuthorDialog(
                                    author.authorId!,
                                    author.name ?? "",
                                  );
                                },
                              ),
                              IconButton(
                                icon: const Icon(Icons.delete),
                                tooltip: "Delete author",
                                onPressed: () async {
                                  await _showDeleteAuthorDialog(
                                    author.authorId!,
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
    if (_authors == null || _authors!.totalPages <= 1) {
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
                    await _fetchAuthors();
                  }
                : null,
          ),
          Text("Page $_currentPage of ${_authors!.totalPages}"),
          IconButton(
            icon: const Icon(Icons.chevron_right),
            onPressed: _currentPage < _authors!.totalPages
                ? () async {
                    _isLoading = true;
                    _currentPage += 1;
                    await _fetchAuthors();
                  }
                : null,
          ),
        ],
      ),
    );
  }
}
