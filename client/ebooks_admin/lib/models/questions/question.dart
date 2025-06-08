import "package:ebooks_admin/models/users/user.dart";
import "package:json_annotation/json_annotation.dart";

part "question.g.dart";

@JsonSerializable()
class Question {
  int? questionId;
  User? user;
  String? question1;
  String? answer;
  User? answeredBy;

  Question({this.questionId, this.user, this.question1, this.answer, this.answeredBy});

  factory Question.fromJson(Map<String, dynamic> json) => _$QuestionFromJson(json);
  Map<String, dynamic> toJson() => _$QuestionToJson(this);
}
