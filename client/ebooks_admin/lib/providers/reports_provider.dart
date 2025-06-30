import "package:ebooks_admin/models/reports/report.dart";
import "package:ebooks_admin/providers/base_provider.dart";
import "package:ebooks_admin/utils/globals.dart";
import "package:http/http.dart" as http;

class ReportsProvider extends BaseProvider<Report> {
  ReportsProvider() : super("reports");

  @override
  Report fromJson(data) {
    return Report.fromJson(data);
  }

  Future adminDelete(int userId, int bookId) async {
    var uri = Uri.parse("${Globals.apiAddress}/reports/$userId/$bookId");
    var headers = createHeaders();
    var response = await http.delete(uri, headers: headers);
    if (!isValidResponse(response)) {
      throw response.body;
    }
  }
}
