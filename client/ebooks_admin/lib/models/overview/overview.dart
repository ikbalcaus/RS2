import 'package:json_annotation/json_annotation.dart';

part 'overview.g.dart';

@JsonSerializable()
class Overview {
  int? booksCount;
  int? approvedBooksCount;
  int? awaitedBooksCount;
  int? draftedCount;
  int? hiddenCount;
  int? rejectedCount;
  int? usersCount;
  int? authorsCount;
  int? genresCount;
  int? languagesCount;
  int? questionsCount;
  int? answeredQuestionsCount;
  int? unansweredQuestionsCount;
  int? reportsCount;
  int? reviewsCount;
  int? purchasesCount;

  Overview({
    this.booksCount,
    this.approvedBooksCount,
    this.awaitedBooksCount,
    this.draftedCount,
    this.hiddenCount,
    this.rejectedCount,
    this.usersCount,
    this.authorsCount,
    this.genresCount,
    this.languagesCount,
    this.questionsCount,
    this.answeredQuestionsCount,
    this.unansweredQuestionsCount,
    this.reportsCount,
    this.reviewsCount,
    this.purchasesCount,
  });

  factory Overview.fromJson(Map<String, dynamic> json) =>
      _$OverviewFromJson(json);
  Map<String, dynamic> toJson() => _$OverviewToJson(this);
}
