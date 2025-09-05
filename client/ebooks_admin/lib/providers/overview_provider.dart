import "dart:convert";
import "dart:io";
import "package:ebooks_admin/models/overview/overview.dart";
import "package:ebooks_admin/providers/base_provider.dart";
import "package:ebooks_admin/utils/globals.dart";
import "package:http/http.dart" as http;

class OverviewProvider extends BaseProvider<Overview> {
  OverviewProvider() : super("overview");

  @override
  Overview fromJson(data) {
    return Overview.fromJson(data);
  }

  Future<Overview> getAllCount() async {
    try {
      var uri = Uri.parse("${Globals.apiAddress}/overview");
      var headers = createHeaders();
      var response = await http.get(uri, headers: headers);

      if (response.statusCode == 200) {
        var data = jsonDecode(response.body);
        return Overview.fromJson(data);
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
