import "package:ebooks_user/models/wishlist/wishlist.dart";
import "package:ebooks_user/providers/base_provider.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:http/http.dart" as http;

class WishlistProvider extends BaseProvider<Wishlist> {
  WishlistProvider() : super("wishlist");

  @override
  Wishlist fromJson(data) {
    return Wishlist.fromJson(data);
  }

  Future patch(int id) async {
    var uri = Uri.parse("${Globals.apiAddress}/wishlist/$id");
    var headers = createHeaders();
    var response = await http.patch(uri, headers: headers);
    if (!isValidResponse(response)) {
      throw response.body;
    }
  }
}
