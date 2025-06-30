import "package:ebooks_user/models/users/user.dart";
import "package:json_annotation/json_annotation.dart";

part "report.g.dart";

@JsonSerializable()
class Report {
  User? user;
  int? bookId;
  DateTime? modifiedAt;
  String? reason;

  Report({this.user, this.bookId, this.modifiedAt, this.reason});

  factory Report.fromJson(Map<String, dynamic> json) => _$ReportFromJson(json);
  Map<String, dynamic> toJson() => _$ReportToJson(this);
}
