// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'report.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Report _$ReportFromJson(Map<String, dynamic> json) => Report(
  user: json['user'] == null
      ? null
      : User.fromJson(json['user'] as Map<String, dynamic>),
  bookId: (json['bookId'] as num?)?.toInt(),
  modifiedAt: json['modifiedAt'] == null
      ? null
      : DateTime.parse(json['modifiedAt'] as String),
  reason: json['reason'] as String?,
);

Map<String, dynamic> _$ReportToJson(Report instance) => <String, dynamic>{
  'user': instance.user,
  'bookId': instance.bookId,
  'modifiedAt': instance.modifiedAt?.toIso8601String(),
  'reason': instance.reason,
};
