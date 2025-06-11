import "package:ebooks_user/models/users/user.dart";
import "package:ebooks_user/providers/base_provider.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:http/http.dart" as http;

class UsersProvider extends BaseProvider<User> {
  UsersProvider() : super("users");

  @override
  User fromJson(data) {
    return User.fromJson(data);
  }

  Future adminDelete(int id, String? reason) async {
    var uri = Uri.parse("${Globals.apiAddress}/users/$id/admin-delete");
    if (reason != null && reason.isNotEmpty) {
      uri = uri.replace(queryParameters: {"reason": reason});
    }
    var headers = createHeaders();
    var response = await http.delete(uri, headers: headers);
    if (!isValidResponse(response)) {
      throw response.body;
    }
  }

  Future verifyPublisher(int id) async {
    var uri = Uri.parse("${Globals.apiAddress}/users/$id/verify-publisher");
    var headers = createHeaders();
    var response = await http.patch(uri, headers: headers);
    if (!isValidResponse(response)) {
      throw response.body;
    }
  }
}
