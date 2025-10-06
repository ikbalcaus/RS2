import "dart:convert";
import "dart:io";
import "package:ebooks_user/models/purchases/purchase.dart";
import "package:ebooks_user/models/search_result.dart";
import "package:ebooks_user/providers/base_provider.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:http/http.dart" as http;

class PurchasesProvider extends BaseProvider<Purchase> {
  PurchasesProvider() : super("purchases");

  @override
  Purchase fromJson(data) {
    return Purchase.fromJson(data);
  }

  Future<SearchResult<Purchase>> getAllByPublisherId(
    int publisherId, {
    int page = 1,
    int pageSize = 10,
    Map<String, dynamic>? filter,
  }) async {
    try {
      var url = "${Globals.apiAddress}/purchases/$publisherId/payment-history";
      final queryParams = <String, String>{};
      queryParams["Page"] = page.toString();
      queryParams["PageSize"] = pageSize.toString();
      if (filter != null) {
        filter.forEach((key, value) {
          if (value != null && value.toString().isNotEmpty) {
            queryParams[key] = value.toString();
          }
        });
      }
      var uri = Uri.parse(url).replace(queryParameters: queryParams);
      var headers = createHeaders();
      var response = await http.get(uri, headers: headers);
      if (isValidResponse(response)) {
        var data = jsonDecode(response.body);
        var result = SearchResult<Purchase>();
        result.count = data["count"];
        for (var item in data["resultList"]) {
          result.resultList.add(fromJson(item));
        }
        result.totalPages = (result.count / pageSize).ceil();
        return result;
      } else {
        throw response.body;
      }
    } on SocketException {
      throw "No internet connection";
    } catch (ex) {
      throw ex.toString();
    }
  }
}
