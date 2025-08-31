import "package:ebooks_user/models/purchases/purchase.dart";
import "package:ebooks_user/models/search_result.dart";
import "package:ebooks_user/providers/purchases_provider.dart";
import "package:ebooks_user/screens/master_screen.dart";
import "package:ebooks_user/screens/publisher_screen.dart";
import "package:ebooks_user/utils/helpers.dart";
import "package:ebooks_user/widgets/not_logged_in_view.dart";
import "package:flutter/material.dart";
import "package:ebooks_user/providers/auth_provider.dart";
import "package:provider/provider.dart";

class PaymentHistoryScreen extends StatefulWidget {
  final int publisherId;
  const PaymentHistoryScreen({super.key, required this.publisherId});

  @override
  State<PaymentHistoryScreen> createState() => _PaymentHistoryScreenState();
}

class _PaymentHistoryScreenState extends State<PaymentHistoryScreen> {
  late PurchasesProvider _purchasesProvider;
  SearchResult<Purchase>? _purchases;
  final int _currentPage = 1;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    if (AuthProvider.isLoggedIn) {
      _purchasesProvider = context.read<PurchasesProvider>();
      _fetchPaymentHistory();
    }
  }

  @override
  Widget build(BuildContext context) {
    Widget content;
    if (!AuthProvider.isLoggedIn) {
      content = Center(child: NotLoggedInView());
    } else if (_isLoading) {
      content = const Center(child: CircularProgressIndicator());
    } else if (_purchases?.count == 0) {
      content = const Center(child: Text("Your payment history is empty"));
    } else {
      content = _buildResultView();
    }
    return MasterScreen(child: content);
  }

  Future _fetchPaymentHistory() async {
    setState(() => _isLoading = true);
    try {
      final purchases = await _purchasesProvider.getAllByPublisherId(
        widget.publisherId,
        page: _currentPage,
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

  Widget _buildResultView() {
    final items = _purchases?.resultList ?? [];
    return ListView.builder(
      itemCount: items.length,
      itemBuilder: (context, index) {
        final purchase = items[index];
        final isLast = index == items.length - 1;
        return Column(
          children: [
            ListTile(
              title: Text(purchase.book?.title ?? ""),
              subtitle: Text(
                purchase.user!.userId == widget.publisherId
                    ? purchase.publisher?.userName ?? ""
                    : purchase.user?.userName ?? "",
              ),
              trailing: Text(
                purchase.user!.userId == widget.publisherId
                    ? "-${purchase.totalPrice?.toStringAsFixed(2)}€"
                    : "+${purchase.totalPrice?.toStringAsFixed(2)}€",
                style: TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.w700,
                  color: purchase.user!.userId == widget.publisherId
                      ? Colors.red
                      : Colors.green,
                ),
              ),
              onTap: () => Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (context) => PublisherScreen(
                    publisherId: purchase.user!.userId == widget.publisherId
                        ? purchase.publisher!.userId!
                        : purchase.user!.userId!,
                  ),
                ),
              ),
            ),
            const Divider(height: 1),
            if (isLast) const Divider(height: 0.1),
          ],
        );
      },
    );
  }
}
