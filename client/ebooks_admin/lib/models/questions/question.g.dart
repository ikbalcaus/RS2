// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'question.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Question _$QuestionFromJson(Map<String, dynamic> json) => Question(
  questionId: (json['questionId'] as num?)?.toInt(),
  user: json['user'] == null
      ? null
      : User.fromJson(json['user'] as Map<String, dynamic>),
  question1: json['question1'] as String?,
  answer: json['answer'] as String?,
  answeredBy: json['answeredBy'] == null
      ? null
      : User.fromJson(json['answeredBy'] as Map<String, dynamic>),
);

Map<String, dynamic> _$QuestionToJson(Question instance) => <String, dynamic>{
  'questionId': instance.questionId,
  'user': instance.user,
  'question1': instance.question1,
  'answer': instance.answer,
  'answeredBy': instance.answeredBy,
};
