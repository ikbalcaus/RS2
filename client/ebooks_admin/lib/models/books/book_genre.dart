import 'package:ebooks_admin/models/genres/genre.dart';
import 'package:json_annotation/json_annotation.dart';

part 'book_genre.g.dart';

@JsonSerializable()
class BookGenre {
  int? bookId;
  Genre? genre;

  BookGenre({this.bookId, this.genre});

  factory BookGenre.fromJson(Map<String, dynamic> json) =>
      _$BookGenreFromJson(json);
  Map<String, dynamic> toJson() => _$BookGenreToJson(this);
}
