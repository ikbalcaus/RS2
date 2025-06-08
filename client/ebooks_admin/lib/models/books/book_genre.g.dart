// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'book_genre.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

BookGenre _$BookGenreFromJson(Map<String, dynamic> json) => BookGenre(
  bookId: (json['bookId'] as num?)?.toInt(),
  genre: json['genre'] == null
      ? null
      : Genre.fromJson(json['genre'] as Map<String, dynamic>),
);

Map<String, dynamic> _$BookGenreToJson(BookGenre instance) => <String, dynamic>{
  'bookId': instance.bookId,
  'genre': instance.genre,
};
