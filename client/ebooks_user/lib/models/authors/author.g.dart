// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'author.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Author _$AuthorFromJson(Map<String, dynamic> json) => Author(
  authorId: (json['authorId'] as num?)?.toInt(),
  name: json['name'] as String?,
  modifiedBy: json['modifiedBy'] == null
      ? null
      : User.fromJson(json['modifiedBy'] as Map<String, dynamic>),
);

Map<String, dynamic> _$AuthorToJson(Author instance) => <String, dynamic>{
  'authorId': instance.authorId,
  'name': instance.name,
  'modifiedBy': instance.modifiedBy,
};
