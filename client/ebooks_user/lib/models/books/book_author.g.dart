// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'book_author.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

BookAuthor _$BookAuthorFromJson(Map<String, dynamic> json) => BookAuthor(
  bookId: (json['bookId'] as num?)?.toInt(),
  author: json['author'] == null
      ? null
      : Author.fromJson(json['author'] as Map<String, dynamic>),
);

Map<String, dynamic> _$BookAuthorToJson(BookAuthor instance) =>
    <String, dynamic>{'bookId': instance.bookId, 'author': instance.author};
