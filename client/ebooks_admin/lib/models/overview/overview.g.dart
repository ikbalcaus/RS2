// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'overview.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Overview _$OverviewFromJson(Map<String, dynamic> json) => Overview(
  booksCount: (json['booksCount'] as num?)?.toInt(),
  approvedBooksCount: (json['approvedBooksCount'] as num?)?.toInt(),
  awaitedBooksCount: (json['awaitedBooksCount'] as num?)?.toInt(),
  draftedCount: (json['draftedCount'] as num?)?.toInt(),
  hiddenCount: (json['hiddenCount'] as num?)?.toInt(),
  rejectedCount: (json['rejectedCount'] as num?)?.toInt(),
  usersCount: (json['usersCount'] as num?)?.toInt(),
  authorsCount: (json['authorsCount'] as num?)?.toInt(),
  genresCount: (json['genresCount'] as num?)?.toInt(),
  languagesCount: (json['languagesCount'] as num?)?.toInt(),
  questionsCount: (json['questionsCount'] as num?)?.toInt(),
  answeredQuestionsCount: (json['answeredQuestionsCount'] as num?)?.toInt(),
  unansweredQuestionsCount: (json['unansweredQuestionsCount'] as num?)?.toInt(),
  reportsCount: (json['reportsCount'] as num?)?.toInt(),
  reviewsCount: (json['reviewsCount'] as num?)?.toInt(),
  purchasesCount: (json['purchasesCount'] as num?)?.toInt(),
);

Map<String, dynamic> _$OverviewToJson(Overview instance) => <String, dynamic>{
  'booksCount': instance.booksCount,
  'approvedBooksCount': instance.approvedBooksCount,
  'awaitedBooksCount': instance.awaitedBooksCount,
  'draftedCount': instance.draftedCount,
  'hiddenCount': instance.hiddenCount,
  'rejectedCount': instance.rejectedCount,
  'usersCount': instance.usersCount,
  'authorsCount': instance.authorsCount,
  'genresCount': instance.genresCount,
  'languagesCount': instance.languagesCount,
  'questionsCount': instance.questionsCount,
  'answeredQuestionsCount': instance.answeredQuestionsCount,
  'unansweredQuestionsCount': instance.unansweredQuestionsCount,
  'reportsCount': instance.reportsCount,
  'reviewsCount': instance.reviewsCount,
  'purchasesCount': instance.purchasesCount,
};
