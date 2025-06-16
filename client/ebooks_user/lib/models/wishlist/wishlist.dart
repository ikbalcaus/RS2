import "package:ebooks_user/models/books/book.dart";
import "package:json_annotation/json_annotation.dart";

part "wishlist.g.dart";

@JsonSerializable()
class Wishlist {
  int? userId;
  Book? book;
  DateTime? modifiedAt;

  Wishlist({this.userId, this.book, this.modifiedAt});

  factory Wishlist.fromJson(Map<String, dynamic> json) =>
      _$WishlistFromJson(json);
  Map<String, dynamic> toJson() => _$WishlistToJson(this);
}
