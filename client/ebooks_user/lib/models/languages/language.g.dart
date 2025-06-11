// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'language.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Language _$LanguageFromJson(Map<String, dynamic> json) => Language(
  languageId: (json['languageId'] as num?)?.toInt(),
  name: json['name'] as String?,
  modifiedBy: json['modifiedBy'] == null
      ? null
      : User.fromJson(json['modifiedBy'] as Map<String, dynamic>),
);

Map<String, dynamic> _$LanguageToJson(Language instance) => <String, dynamic>{
  'languageId': instance.languageId,
  'name': instance.name,
  'modifiedBy': instance.modifiedBy,
};
