import "package:ebooks_admin/models/users/user.dart";
import "package:ebooks_admin/providers/base_provider.dart";
import "package:ebooks_admin/utils/constants.dart";
import "package:http/http.dart" as http;

class UsersProvider extends BaseProvider<User> {
  UsersProvider() : super("users");

  @override
  User fromJson(data) {
    return User.fromJson(data);
  }

  Future adminDelete(int id, String? reason) async {
    var uri = Uri.parse("${Constants.apiAddress}/users/$id/admin-delete");
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
    var uri = Uri.parse("${Constants.apiAddress}/users/$id/verify-publisher");
    var headers = createHeaders();
    var response = await http.patch(uri, headers: headers);
    if (!isValidResponse(response)) {
      throw response.body;
    }
  }
}
