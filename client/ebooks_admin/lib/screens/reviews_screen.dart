import "package:ebooks_admin/models/reviews/review.dart";
import "package:ebooks_admin/models/search_result.dart";
import "package:ebooks_admin/providers/reviews_provider.dart";
import "package:ebooks_admin/screens/book_details_screen.dart";
import "package:ebooks_admin/screens/master_screen.dart";
import "package:ebooks_admin/screens/users_screen.dart";
import "package:ebooks_admin/utils/globals.dart";
import "package:ebooks_admin/utils/helpers.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";

class ReviewsScreen extends StatefulWidget {
  const ReviewsScreen({super.key});

  @override
  State<ReviewsScreen> createState() => _ReviewsScreenState();
}

class _ReviewsScreenState extends State<ReviewsScreen> {
  late ReviewsProvider _reviewsProvider;
  SearchResult<Review>? _reviews;
  bool _isLoading = true;
  int _currentPage = 1;
  String _isReported = "All reviews";
  String _orderBy = "Last added";
  Map<String, dynamic> _currentFilter = {};
  final TextEditingController _reviewedByController = TextEditingController();
  final TextEditingController _bookTitleController = TextEditingController();

  @override
  void initState() {
    super.initState();
    _currentFilter = {"isBookIncluded": true, "IsReportedByIncluded": true};
    _reviewsProvider = context.read<ReviewsProvider>();
    _fetchReviews();
  }

  @override
  void dispose() {
    _reviewedByController.dispose();
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

  Future _fetchReviews() async {
    setState(() {
      _isLoading = true;
    });
    try {
      final reviews = await _reviewsProvider.getPaged(
        page: _currentPage,
        filter: _currentFilter,
      );
      if (!mounted) return;
      setState(() => _reviews = reviews);
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

  Future _showDeleteReviewDialog(int userId, int bookId) async {
    await showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: const Text("Confirm delete"),
          actions: [
            TextButton(
              onPressed: () async {
                try {
                  await _reviewsProvider.adminDelete(userId, bookId);
                  await _fetchReviews();
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
              controller: _reviewedByController,
              decoration: const InputDecoration(labelText: "Review"),
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
              value: _isReported,
              onChanged: (value) {
                _isReported = value!;
              },
              items: ["All reviews", "Reported", "Not reported"].map((
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
                "ReviewedBy": _reviewedByController.text,
                "BookTitle": _bookTitleController.text,
                "IsReported": _isReported,
                "OrderBy": _orderBy,
                "isBookIncluded": true,
                "IsReportedByIncluded": true,
              };
              await _fetchReviews();
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
            DataColumn(label: Text("Reviewed by")),
            DataColumn(label: Text("Book")),
            DataColumn(label: Text("Rating")),
            DataColumn(label: Text("Comment")),
            DataColumn(label: Text("Reported by")),
            DataColumn(label: Text("Report reason")),
            DataColumn(label: Text("Actions")),
          ],
          rows:
              _reviews?.resultList
                  .map(
                    (review) => DataRow(
                      cells: [
                        DataCell(Text(review.user?.userName ?? "")),
                        DataCell(Text(review.book?.title ?? "")),
                        DataCell(Text(review.rating.toString())),
                        DataCell(
                          SizedBox(
                            width: 180,
                            child: Text(review.comment ?? "", softWrap: true),
                          ),
                        ),
                        DataCell(Text(review.reportedBy?.userName ?? "")),
                        DataCell(
                          SizedBox(
                            width: 180,
                            child: Text(
                              review.reportReason ?? "",
                              softWrap: true,
                            ),
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
                                      userName: review.user?.userName,
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
                                      bookId: review.book!.bookId!,
                                    ),
                                  ),
                                ),
                              ),
                              IconButton(
                                icon: const Icon(Icons.delete),
                                tooltip: "Delete review",
                                onPressed: () async {
                                  await _showDeleteReviewDialog(
                                    review.user!.userId!,
                                    review.book!.bookId!,
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
    if (_reviews == null || _reviews!.totalPages <= 1) {
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
                    await _fetchReviews();
                  }
                : null,
          ),
          Text("Page $_currentPage of ${_reviews!.totalPages}"),
          IconButton(
            icon: const Icon(Icons.chevron_right),
            onPressed: _currentPage < _reviews!.totalPages
                ? () async {
                    _isLoading = true;
                    _currentPage += 1;
                    await _fetchReviews();
                  }
                : null,
          ),
        ],
      ),
    );
  }
}
