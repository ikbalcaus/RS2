import "package:ebooks_user/models/reviews/review.dart";
import "package:ebooks_user/providers/base_provider.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:http/http.dart" as http;

class ReviewsProvider extends BaseProvider<Review> {
  ReviewsProvider() : super("reviews");

  @override
  Review fromJson(data) {
    return Review.fromJson(data);
  }

  Future report(int userId, int bookId, String reason) async {
    var uri = Uri.parse("${Globals.apiAddress}/reviews/report/$userId/$bookId");
    uri = uri.replace(queryParameters: {"reason": reason});
    var headers = createHeaders();
    var response = await http.patch(uri, headers: headers);
    if (!isValidResponse(response)) {
      throw response.body;
    }
  }
}
