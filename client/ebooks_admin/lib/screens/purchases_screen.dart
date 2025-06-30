import "package:ebooks_admin/models/purchases/purchase.dart";
import "package:ebooks_admin/models/search_result.dart";
import "package:ebooks_admin/providers/purchases_provider.dart";
import "package:ebooks_admin/screens/master_screen.dart";
import "package:ebooks_admin/utils/globals.dart";
import "package:ebooks_admin/utils/helpers.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";

class PurchasesScreen extends StatefulWidget {
  const PurchasesScreen({super.key});

  @override
  State<PurchasesScreen> createState() => _PurchasesScreenState();
}

class _PurchasesScreenState extends State<PurchasesScreen> {
  late PurchasesProvider _purchasesProvider;
  SearchResult<Purchase>? _purchases;
  bool _isLoading = true;
  int _currentPage = 1;
  String _orderBy = "Last created";
  Map<String, dynamic> _currentFilter = {};
  final TextEditingController _userController = TextEditingController();
  final TextEditingController _publisherController = TextEditingController();
  final TextEditingController _bookController = TextEditingController();

  @override
  void initState() {
    super.initState();
    _purchasesProvider = context.read<PurchasesProvider>();
    _fetchPurchases();
  }

  @override
  void dispose() {
    _userController.dispose();
    _publisherController.dispose();
    _bookController.dispose();
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

  Future _fetchPurchases() async {
    setState(() => _isLoading = true);
    try {
      final purchases = await _purchasesProvider.getPaged(
        page: _currentPage,
        filter: _currentFilter,
      );
      if (!mounted) return;
      setState(() => _purchases = purchases);
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

  Widget _buildSearch() {
    return Padding(
      padding: const EdgeInsets.all(Globals.spacing),
      child: Row(
        children: [
          Expanded(
            child: TextField(
              controller: _userController,
              decoration: const InputDecoration(labelText: "User"),
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
              controller: _bookController,
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
              items:
                  [
                    "Last created",
                    "First created",
                    "User (A-Z)",
                    "User (Z-A)",
                    "Publisher (A-Z)",
                    "Publisher (Z-A)",
                    "Book (Z-A)",
                    "Book (A-Z)",
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
                "User": _userController.text,
                "Publisher": _publisherController.text,
                "Book": _bookController.text,
                "OrderBy": _orderBy,
              };
              await _fetchPurchases();
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
            DataColumn(label: Text("User")),
            DataColumn(label: Text("Publisher")),
            DataColumn(label: Text("Book")),
            DataColumn(label: Text("Created at")),
            DataColumn(label: Text("Total price")),
            DataColumn(label: Text("Payment status")),
            DataColumn(label: Text("Payment method")),
          ],
          rows:
              _purchases?.resultList
                  .map(
                    (purchase) => DataRow(
                      cells: [
                        DataCell(Text(purchase.user?.userName ?? "")),
                        DataCell(Text(purchase.publisher?.userName ?? "")),
                        DataCell(Text(purchase.book?.title ?? "")),
                        DataCell(Text(purchase.createdAt.toString())),
                        DataCell(Text("€${purchase.totalPrice.toString()}")),
                        DataCell(Text(purchase.paymentStatus ?? "")),
                        DataCell(Text(purchase.paymentMethod ?? "")),
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
    if (_purchases == null || _purchases!.totalPages <= 1) {
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
                    await _fetchPurchases();
                  }
                : null,
          ),
          Text("Page $_currentPage of ${_purchases!.totalPages}"),
          IconButton(
            icon: const Icon(Icons.chevron_right),
            onPressed: _currentPage < _purchases!.totalPages
                ? () async {
                    _isLoading = true;
                    _currentPage += 1;
                    await _fetchPurchases();
                  }
                : null,
          ),
        ],
      ),
    );
  }
}
