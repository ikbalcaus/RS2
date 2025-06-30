import "package:ebooks_user/models/users/user.dart";
import "package:json_annotation/json_annotation.dart";

part "review.g.dart";

@JsonSerializable()
class Review {
  User? user;
  int? bookId;
  DateTime? modifiedAt;
  int? rating;
  String? comment;
  int? reportedById;

  Review({
    this.user,
    this.bookId,
    this.modifiedAt,
    this.rating,
    this.comment,
    this.reportedById,
  });

  factory Review.fromJson(Map<String, dynamic> json) => _$ReviewFromJson(json);
  Map<String, dynamic> toJson() => _$ReviewToJson(this);
}
