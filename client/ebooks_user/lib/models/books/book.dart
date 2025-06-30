import "package:ebooks_user/models/authors/author.dart";
import "package:ebooks_user/models/books/book_author.dart";
import "package:ebooks_user/models/books/book_genre.dart";
import "package:ebooks_user/models/genres/genre.dart";
import "package:ebooks_user/models/languages/language.dart";
import "package:ebooks_user/models/users/user.dart";
import "package:json_annotation/json_annotation.dart";

part "book.g.dart";

@JsonSerializable()
class Book {
  int? bookId;
  String? title;
  String? description;
  String? filePath;
  double? price;
  int? numberOfPages;
  int? numberOfViews;
  String? status;
  String? rejectionReason;
  String? deletionReason;
  bool? accessRightExist;
  DateTime? modifiedAt;
  int? discountPercentage;
  DateTime? discountStart;
  DateTime? discountEnd;
  User? publisher;
  Language? language;
  List<BookAuthor>? bookAuthors;
  List<BookGenre>? bookGenres;
  double? averageRating;

  List<Author>? get authors =>
      bookAuthors?.map((bookAuthor) => bookAuthor.author!).toList();
  List<Genre>? get genres =>
      bookGenres?.map((bookGenre) => bookGenre.genre!).toList();

  Book({
    this.bookId,
    this.title,
    this.description,
    this.filePath,
    this.price,
    this.numberOfPages,
    this.numberOfViews,
    this.status,
    this.rejectionReason,
    this.deletionReason,
    this.accessRightExist,
    this.modifiedAt,
    this.discountPercentage,
    this.discountStart,
    this.discountEnd,
    this.publisher,
    this.language,
    this.bookAuthors,
    this.bookGenres,
  });

  factory Book.fromJson(Map<String, dynamic> json) => _$BookFromJson(json);
  Map<String, dynamic> toJson() => _$BookToJson(this);
}
