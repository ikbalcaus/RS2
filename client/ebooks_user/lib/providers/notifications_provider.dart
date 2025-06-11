import "package:ebooks_user/models/notifications/app_notification.dart";
import "package:ebooks_user/providers/base_provider.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:http/http.dart" as http;

class NotificationsProvider extends BaseProvider<AppNotification> {
  NotificationsProvider() : super("notifications");

  @override
  AppNotification fromJson(data) {
    return AppNotification.fromJson(data);
  }

  Future markAsRead(int id) async {
    var uri = Uri.parse("${Globals.apiAddress}/notifications/$id");
    var headers = createHeaders();
    var response = await http.patch(uri, headers: headers);
    if (!isValidResponse(response)) {
      throw response.body;
    }
  }
}
