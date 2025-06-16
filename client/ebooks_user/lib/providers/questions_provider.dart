import "package:ebooks_user/models/questions/question.dart";
import "package:ebooks_user/providers/base_provider.dart";

class QuestionsProvider extends BaseProvider<Question> {
  QuestionsProvider() : super("questions");

  @override
  Question fromJson(data) {
    return Question.fromJson(data);
  }
}
