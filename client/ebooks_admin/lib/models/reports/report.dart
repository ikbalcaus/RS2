import "package:ebooks_admin/models/books/book.dart";
import "package:ebooks_admin/models/users/user.dart";
import "package:json_annotation/json_annotation.dart";

part "report.g.dart";

@JsonSerializable()
class Report {
  User? user;
  Book? book;
  DateTime? modifiedAt;
  String? reason;

  Report({this.user, this.book, this.modifiedAt, this.reason});

  factory Report.fromJson(Map<String, dynamic> json) => _$ReportFromJson(json);
  Map<String, dynamic> toJson() => _$ReportToJson(this);
}
