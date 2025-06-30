import "package:ebooks_user/models/reports/report.dart";
import "package:ebooks_user/providers/base_provider.dart";

class ReportsProvider extends BaseProvider<Report> {
  ReportsProvider() : super("reports");

  @override
  Report fromJson(data) {
    return Report.fromJson(data);
  }
}
