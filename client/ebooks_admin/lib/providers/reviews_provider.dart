import "package:ebooks_admin/models/reviews/review.dart";
import "package:ebooks_admin/providers/base_provider.dart";
import "package:ebooks_admin/utils/globals.dart";
import "package:http/http.dart" as http;

class ReviewsProvider extends BaseProvider<Review> {
  ReviewsProvider() : super("reviews");

  @override
  Review fromJson(data) {
    return Review.fromJson(data);
  }

  Future adminDelete(int userId, int bookId) async {
    var uri = Uri.parse("${Globals.apiAddress}/reviews/$userId/$bookId");
    var headers = createHeaders();
    var response = await http.delete(uri, headers: headers);
    if (!isValidResponse(response)) {
      throw response.body;
    }
  }
}
