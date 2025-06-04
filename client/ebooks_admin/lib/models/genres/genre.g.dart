// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'genre.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Genre _$GenreFromJson(Map<String, dynamic> json) => Genre(
  genreId: (json['genreId'] as num?)?.toInt(),
  name: json['name'] as String?,
  modifiedBy: json['modifiedBy'] == null
      ? null
      : User.fromJson(json['modifiedBy'] as Map<String, dynamic>),
);

Map<String, dynamic> _$GenreToJson(Genre instance) => <String, dynamic>{
  'genreId': instance.genreId,
  'name': instance.name,
  'modifiedBy': instance.modifiedBy,
};
