import 'package:ebooks_user/models/authors/author.dart';
import 'package:json_annotation/json_annotation.dart';

part 'book_author.g.dart';

@JsonSerializable()
class BookAuthor {
  int? bookId;
  Author? author;

  BookAuthor({this.bookId, this.author});

  factory BookAuthor.fromJson(Map<String, dynamic> json) =>
      _$BookAuthorFromJson(json);
  Map<String, dynamic> toJson() => _$BookAuthorToJson(this);
}
