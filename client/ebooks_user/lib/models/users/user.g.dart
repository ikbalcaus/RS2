// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'user.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

User _$UserFromJson(Map<String, dynamic> json) => User(
  userId: (json['userId'] as num?)?.toInt(),
  firstName: json['firstName'] as String?,
  lastName: json['lastName'] as String?,
  userName: json['userName'] as String?,
  email: json['email'] as String?,
  filePath: json['filePath'] as String?,
  deletionReason: json['deletionReason'] as String?,
  isEmailVerified: json['isEmailVerified'] as bool?,
  publisherVerifiedById: (json['publisherVerifiedById'] as num?)?.toInt(),
);

Map<String, dynamic> _$UserToJson(User instance) => <String, dynamic>{
  'userId': instance.userId,
  'firstName': instance.firstName,
  'lastName': instance.lastName,
  'userName': instance.userName,
  'email': instance.email,
  'filePath': instance.filePath,
  'deletionReason': instance.deletionReason,
  'isEmailVerified': instance.isEmailVerified,
  'publisherVerifiedById': instance.publisherVerifiedById,
};
