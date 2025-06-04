import "package:ebooks_admin/models/genres/genre.dart";
import "package:ebooks_admin/models/search_result.dart";
import "package:ebooks_admin/providers/genres_provider.dart";
import "package:ebooks_admin/screens/master_screen.dart";
import "package:ebooks_admin/utils/constants.dart";
import "package:ebooks_admin/utils/helpers.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";

class GenresScreen extends StatefulWidget {
  const GenresScreen({super.key});

  @override
  State<GenresScreen> createState() => _GenresScreenState();
}

class _GenresScreenState extends State<GenresScreen> {
  late GenresProvider _genresProvider;
  SearchResult<Genre>? genres;
  bool _isLoading = true;
  int _currentPage = 1;
  Map<String, dynamic> _currentFilter = {};
  String _orderBy = "Last modified";

  final TextEditingController _nameEditingController = TextEditingController();

  @override
  void initState() {
    super.initState();
    _genresProvider = context.read<GenresProvider>();
    fetchGenres();
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

  Future fetchGenres() async {
    setState(() {
      _isLoading = true;
    });
    try {
      final genres = await _genresProvider.getPaged(
        page: _currentPage,
        filter: _currentFilter,
      );
      if (!mounted) {
        return;
      }
      setState(() {
        this.genres = genres;
      });
    } catch (ex) {
      if (!mounted) return;
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (!mounted) {
          return;
        }
        Helpers.showErrorMessage(context, ex);
      });
    } finally {
      if (!mounted) {
        return;
      }
      setState(() {
        _isLoading = false;
      });
    }
  }

  Future _editGenreDialog(BuildContext context, int id, String name) async {
    final TextEditingController dialogController = TextEditingController(
      text: name,
    );
    await showDialog(
      context: context,
      builder: (dialogContext) {
        return AlertDialog(
          title: const Text("Confirm edit"),
          content: TextField(
            controller: dialogController,
            decoration: const InputDecoration(labelText: "Enter new name..."),
          ),
          actions: [
            TextButton(
              onPressed: () async {
                if (dialogController.text.trim().isNotEmpty) {
                  Navigator.of(dialogContext).pop(true);
                  await Future.delayed(const Duration(milliseconds: 250));
                  if (dialogController.text.trim().isNotEmpty) {
                    try {
                      await _genresProvider.put(id, {
                        "name": dialogController.text,
                      });
                      Helpers.showSuccessMessage(context);
                      await fetchGenres();
                    } catch (ex) {
                      Helpers.showErrorMessage(context, ex);
                    }
                  }
                }
              },
              child: const Text("Edit"),
            ),
            TextButton(
              onPressed: () async {
                Navigator.of(dialogContext).pop(false);
              },
              child: const Text("Cancel"),
            ),
          ],
        );
      },
    );
  }

  Future _deleteGenreDialog(BuildContext context, int id) async {
    await showDialog(
      context: context,
      builder: (dialogContext) {
        return AlertDialog(
          title: const Text("Confirm delete"),
          actions: [
            TextButton(
              onPressed: () async {
                Navigator.of(dialogContext).pop(true);
                await Future.delayed(const Duration(milliseconds: 250));
                try {
                  await _genresProvider.delete(id);
                  Helpers.showSuccessMessage(context);
                  await fetchGenres();
                } catch (ex) {
                  Helpers.showErrorMessage(context, ex);
                }
              },
              child: const Text("Delete"),
            ),
            TextButton(
              onPressed: () async {
                Navigator.of(dialogContext).pop(false);
              },
              child: const Text("Cancel"),
            ),
          ],
        );
      },
    );
  }

  Widget _buildSearch() {
    return Padding(
      padding: const EdgeInsets.all(Constants.defaultSpacing),
      child: Row(
        children: [
          Expanded(
            child: TextField(
              controller: _nameEditingController,
              decoration: const InputDecoration(labelText: "Name"),
            ),
          ),
          const SizedBox(width: Constants.defaultSpacing),
          Expanded(
            child: DropdownButtonFormField<String>(
              value: _orderBy,
              onChanged: (value) {
                setState(() {
                  _orderBy = value!;
                });
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
          const SizedBox(width: Constants.defaultSpacing),
          ElevatedButton(
            onPressed: () async {
              _currentPage = 1;
              _currentFilter = {
                "Name": _nameEditingController.text,
                "OrderBy": _orderBy,
              };
              await fetchGenres();
            },
            child: const Text("Search"),
          ),
          const SizedBox(width: Constants.defaultSpacing),
          ElevatedButton(
            onPressed: () async {
              try {
                await _genresProvider.post({
                  "name": _nameEditingController.text,
                });
                Helpers.showSuccessMessage(context);
                await fetchGenres();
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
            DataColumn(label: Text("Name")),
            DataColumn(label: Text("Modified by")),
            DataColumn(label: Text("Actions")),
          ],
          rows:
              genres?.resultList
                  .map(
                    (genre) => DataRow(
                      cells: [
                        DataCell(Text(genre.name ?? "")),
                        DataCell(Text(genre.modifiedBy?.userName ?? "")),
                        DataCell(
                          Row(
                            children: [
                              IconButton(
                                icon: const Icon(Icons.edit),
                                tooltip: "Edit genre",
                                onPressed: () async {
                                  await _editGenreDialog(
                                    context,
                                    genre.genreId!,
                                    genre.name ?? "",
                                  );
                                },
                              ),
                              IconButton(
                                icon: const Icon(Icons.delete),
                                tooltip: "Delete genre",
                                onPressed: () async {
                                  await _deleteGenreDialog(
                                    context,
                                    genre.genreId!,
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
    if (genres == null || genres!.totalPages <= 1) {
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
                    await fetchGenres();
                  }
                : null,
          ),
          Text("Page $_currentPage of ${genres!.totalPages}"),
          IconButton(
            icon: const Icon(Icons.chevron_right),
            onPressed: _currentPage < genres!.totalPages
                ? () async {
                    _isLoading = true;
                    _currentPage += 1;
                    await fetchGenres();
                  }
                : null,
          ),
        ],
      ),
    );
  }
}
