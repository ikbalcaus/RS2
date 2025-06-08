import "package:ebooks_admin/models/purchases/purchase.dart";
import "package:ebooks_admin/providers/base_provider.dart";

class PurchasesProvider extends BaseProvider<Purchase> {
  PurchasesProvider() : super("purchases");

  @override
  Purchase fromJson(data) {
    return Purchase.fromJson(data);
  }
}
