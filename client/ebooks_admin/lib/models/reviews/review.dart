import "package:ebooks_admin/models/books/book.dart";
import "package:ebooks_admin/models/users/user.dart";
import "package:json_annotation/json_annotation.dart";

part "review.g.dart";

@JsonSerializable()
class Review {
  User? user;
  Book? book;
  DateTime? modifiedAt;
  int? rating;
  String? comment;
  int? reportedById;

  Review({
    this.user,
    this.book,
    this.modifiedAt,
    this.rating,
    this.comment,
    this.reportedById,
  });

  factory Review.fromJson(Map<String, dynamic> json) => _$ReviewFromJson(json);
  Map<String, dynamic> toJson() => _$ReviewToJson(this);
}
