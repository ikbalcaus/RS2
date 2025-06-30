import "package:ebooks_admin/models/reports/report.dart";
import "package:ebooks_admin/models/search_result.dart";
import "package:ebooks_admin/providers/reports_provider.dart";
import "package:ebooks_admin/screens/book_details_screen.dart";
import "package:ebooks_admin/screens/master_screen.dart";
import "package:ebooks_admin/screens/users_screen.dart";
import "package:ebooks_admin/utils/globals.dart";
import "package:ebooks_admin/utils/helpers.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";

class ReportsScreen extends StatefulWidget {
  const ReportsScreen({super.key});

  @override
  State<ReportsScreen> createState() => _ReportsScreenState();
}

class _ReportsScreenState extends State<ReportsScreen> {
  late ReportsProvider _reportsProvider;
  SearchResult<Report>? _reports;
  bool _isLoading = true;
  int _currentPage = 1;
  String _orderBy = "Last added";
  Map<String, dynamic> _currentFilter = {};
  final TextEditingController _reportedByController = TextEditingController();
  final TextEditingController _bookTitleController = TextEditingController();

  @override
  void initState() {
    super.initState();
    _currentFilter = {"isBookIncluded": true};
    _reportsProvider = context.read<ReportsProvider>();
    _fetchReports();
  }

  @override
  void dispose() {
    _reportedByController.dispose();
    _bookTitleController.dispose();
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

  Future _fetchReports() async {
    setState(() {
      _isLoading = true;
    });
    try {
      final reports = await _reportsProvider.getPaged(
        page: _currentPage,
        filter: _currentFilter,
      );
      if (!mounted) return;
      setState(() => _reports = reports);
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

  Future _showDeleteReportDialog(int userId, int bookId) async {
    await showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: const Text("Confirm delete"),
          actions: [
            TextButton(
              onPressed: () async {
                try {
                  await _reportsProvider.adminDelete(userId, bookId);
                  await _fetchReports();
                  if (context.mounted) {
                    Navigator.pop(context);
                    Helpers.showSuccessMessage(context);
                  }
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
              controller: _reportedByController,
              decoration: const InputDecoration(labelText: "Reported by"),
            ),
          ),
          const SizedBox(width: Globals.spacing),
          Expanded(
            child: TextField(
              controller: _bookTitleController,
              decoration: const InputDecoration(labelText: "Book"),
            ),
          ),
          const SizedBox(width: Globals.spacing),
          Expanded(
            child: DropdownButtonFormField<String>(
              value: _orderBy,
              onChanged: (value) {
                _orderBy = value!;
              },
              items: ["Last added", "First added"].map((String value) {
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
                "ReportedBy": _reportedByController.text,
                "BookTitle": _bookTitleController.text,
                "OrderBy": _orderBy,
                "IsBookIncluded": true,
              };
              await _fetchReports();
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
            DataColumn(label: Text("Reported by")),
            DataColumn(label: Text("Book")),
            DataColumn(label: Text("Reason")),
            DataColumn(label: Text("Actions")),
          ],
          rows:
              _reports?.resultList
                  .map(
                    (report) => DataRow(
                      cells: [
                        DataCell(Text(report.user?.userName ?? "")),
                        DataCell(Text(report.book?.title ?? "")),
                        DataCell(
                          SizedBox(
                            width: 300,
                            child: Text(report.reason ?? "", softWrap: true),
                          ),
                        ),
                        DataCell(
                          Row(
                            children: [
                              IconButton(
                                icon: const Icon(Icons.account_box),
                                tooltip: "View user",
                                onPressed: () => Navigator.push(
                                  context,
                                  MaterialPageRoute(
                                    builder: (context) => UsersScreen(
                                      userName: report.user?.userName,
                                    ),
                                  ),
                                ),
                              ),
                              IconButton(
                                icon: const Icon(Icons.book),
                                tooltip: "View book",
                                onPressed: () => Navigator.push(
                                  context,
                                  MaterialPageRoute(
                                    builder: (context) => BookDetailsScreen(
                                      bookId: report.book!.bookId!,
                                    ),
                                  ),
                                ),
                              ),
                              IconButton(
                                icon: const Icon(Icons.delete),
                                tooltip: "Delete report",
                                onPressed: () async {
                                  await _showDeleteReportDialog(
                                    report.user!.userId!,
                                    report.book!.bookId!,
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
    if (_reports == null || _reports!.totalPages <= 1) {
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
                    await _fetchReports();
                  }
                : null,
          ),
          Text("Page $_currentPage of ${_reports!.totalPages}"),
          IconButton(
            icon: const Icon(Icons.chevron_right),
            onPressed: _currentPage < _reports!.totalPages
                ? () async {
                    _isLoading = true;
                    _currentPage += 1;
                    await _fetchReports();
                  }
                : null,
          ),
        ],
      ),
    );
  }
}
