import "package:ebooks_user/models/books/book.dart";
import "package:json_annotation/json_annotation.dart";

part "access_right.g.dart";

@JsonSerializable()
class AccessRight {
  int? userId;
  Book? book;
  DateTime? modifiedAt;
  bool? isFavorite;
  bool? isHidden;

  AccessRight({
    this.userId,
    this.book,
    this.modifiedAt,
    this.isFavorite,
    this.isHidden,
  });

  factory AccessRight.fromJson(Map<String, dynamic> json) =>
      _$AccessRightFromJson(json);
  Map<String, dynamic> toJson() => _$AccessRightToJson(this);
}
