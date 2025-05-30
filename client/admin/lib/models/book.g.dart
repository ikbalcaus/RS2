part of "book.dart";

Book _$BookFromJson(Map<String, dynamic> json) => Book(
  bookId: (json["bookId"] as num?)?.toInt(),
  title: json["title"] as String?,
);

Map<String, dynamic> _$BookToJson(Book instance) => <String, dynamic>{
  "bookId": instance.bookId,
  "title": instance.title,
};
