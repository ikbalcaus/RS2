import "package:json_annotation/json_annotation.dart";

part "book.g.dart";

@JsonSerializable()
class Book {
  int? bookId;
  String? title;

  Book({this.bookId, this.title});

  factory Book.fromJson(Map<String, dynamic> json) => _$BookFromJson(json);

  Map<String, dynamic> toJson() => _$BookToJson(this);
}
