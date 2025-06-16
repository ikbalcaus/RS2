import "dart:convert";
import "package:ebooks_user/models/stripe/stripe.dart";
import "package:ebooks_user/providers/base_provider.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:http/http.dart" as http;

class StripeProvider extends BaseProvider<Stripe> {
  StripeProvider() : super("stripe");

  @override
  Stripe fromJson(data) {
    return Stripe.fromJson(data);
  }

  Future getPaymentPageLink(int bookId) async {
    var uri = Uri.parse(
      "${Globals.apiAddress}/stripe/$bookId/checkout-session",
    );
    var headers = createHeaders();
    var response = await http.get(uri, headers: headers);
    if (isValidResponse(response)) {
      var data = jsonDecode(response.body);
      return fromJson(data);
    } else {
      throw response.body;
    }
  }

  Future getStripeAccountLink() async {
    var uri = Uri.parse("${Globals.apiAddress}/stripe/stripe-account-link");
    var headers = createHeaders();
    var response = await http.get(uri, headers: headers);
    if (isValidResponse(response)) {
      var data = jsonDecode(response.body);
      return fromJson(data);
    } else {
      throw response.body;
    }
  }
}
