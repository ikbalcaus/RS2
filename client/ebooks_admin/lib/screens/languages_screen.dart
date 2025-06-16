import "package:ebooks_admin/models/languages/language.dart";
import "package:ebooks_admin/models/search_result.dart";
import "package:ebooks_admin/providers/languages_provider.dart";
import "package:ebooks_admin/screens/master_screen.dart";
import "package:ebooks_admin/utils/globals.dart";
import "package:ebooks_admin/utils/helpers.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";

class LanguagesScreen extends StatefulWidget {
  final String? name;
  const LanguagesScreen({super.key, this.name});

  @override
  State<LanguagesScreen> createState() => _LanguagesScreenState();
}

class _LanguagesScreenState extends State<LanguagesScreen> {
  late LanguagesProvider _languagesProvider;
  SearchResult<Language>? _languages;
  bool _isLoading = true;
  int _currentPage = 1;
  String _orderBy = "Last modified";
  Map<String, dynamic> _currentFilter = {};
  final TextEditingController _nameEditingController = TextEditingController();

  @override
  void initState() {
    super.initState();
    _nameEditingController.text = widget.name ?? "";
    _currentFilter = {"name": widget.name ?? ""};
    _languagesProvider = context.read<LanguagesProvider>();
    _fetchLanguages();
  }

  @override
  void dispose() {
    _nameEditingController.dispose();
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

  Future _fetchLanguages() async {
    setState(() {
      _isLoading = true;
    });
    try {
      final languages = await _languagesProvider.getPaged(
        page: _currentPage,
        filter: _currentFilter,
      );
      if (!mounted) return;
      setState(() => _languages = languages);
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

  Future _editLanguageDialog(int id, String name) async {
    final TextEditingController dialogController = TextEditingController(
      text: name,
    );
    await showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: const Text("Confirm edit"),
          content: TextField(
            controller: dialogController,
            decoration: const InputDecoration(labelText: "Enter new name..."),
          ),
          actions: [
            TextButton(
              onPressed: () async {
                final dialogText = dialogController.text.trim();
                if (dialogText.isNotEmpty) {
                  Navigator.pop(context);
                  await Future.delayed(const Duration(milliseconds: 250));
                  try {
                    await _languagesProvider.put(id, {"name": dialogText});
                    Helpers.showSuccessMessage(context);
                    await _fetchLanguages();
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

  Future _deleteLanguageDialog(int id) async {
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
                  await _languagesProvider.delete(id);
                  Helpers.showSuccessMessage(context);
                  await _fetchLanguages();
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
              controller: _nameEditingController,
              decoration: const InputDecoration(labelText: "Language"),
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
                "Name": _nameEditingController.text,
                "OrderBy": _orderBy,
              };
              await _fetchLanguages();
            },
            child: const Text("Search"),
          ),
          const SizedBox(width: Globals.spacing),
          ElevatedButton(
            onPressed: () async {
              try {
                await _languagesProvider.post({
                  "name": _nameEditingController.text,
                });
                Helpers.showSuccessMessage(context);
                await _fetchLanguages();
                _nameEditingController.clear();
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
            DataColumn(label: Text("Language")),
            DataColumn(label: Text("Modified by")),
            DataColumn(label: Text("Actions")),
          ],
          rows:
              _languages?.resultList
                  .map(
                    (language) => DataRow(
                      cells: [
                        DataCell(Text(language.name ?? "")),
                        DataCell(Text(language.modifiedBy?.userName ?? "")),
                        DataCell(
                          Row(
                            children: [
                              IconButton(
                                icon: const Icon(Icons.edit),
                                tooltip: "Edit language",
                                onPressed: () async {
                                  await _editLanguageDialog(
                                    language.languageId!,
                                    language.name ?? "",
                                  );
                                },
                              ),
                              IconButton(
                                icon: const Icon(Icons.delete),
                                tooltip: "Delete language",
                                onPressed: () async {
                                  await _deleteLanguageDialog(
                                    language.languageId!,
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
    if (_languages == null || _languages!.totalPages <= 1) {
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
                    await _fetchLanguages();
                  }
                : null,
          ),
          Text("Page $_currentPage of ${_languages!.totalPages}"),
          IconButton(
            icon: const Icon(Icons.chevron_right),
            onPressed: _currentPage < _languages!.totalPages
                ? () async {
                    _isLoading = true;
                    _currentPage += 1;
                    await _fetchLanguages();
                  }
                : null,
          ),
        ],
      ),
    );
  }
}
