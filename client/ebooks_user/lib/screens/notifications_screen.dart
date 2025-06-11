import "package:ebooks_user/models/notifications/app_notification.dart";
import "package:ebooks_user/models/search_result.dart";
import "package:ebooks_user/providers/notifications_provider.dart";
import "package:ebooks_user/screens/master_screen.dart";
import "package:ebooks_user/utils/helpers.dart";
import "package:ebooks_user/widgets/not_logged_in_view.dart";
import "package:flutter/material.dart";
import "package:ebooks_user/providers/auth_provider.dart";
import "package:provider/provider.dart";

class NotificationsScreen extends StatefulWidget {
  const NotificationsScreen({super.key});

  @override
  State<NotificationsScreen> createState() => _NotificationsScreenState();
}

class _NotificationsScreenState extends State<NotificationsScreen> {
  late NotificationsProvider _notificationsProvider;
  SearchResult<AppNotification>? _notifications;
  int _currentPage = 1;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    if (AuthProvider.isLoggedIn) {
      _notificationsProvider = context.read<NotificationsProvider>();
      _fetchNotifications();
    }
  }

  @override
  Widget build(BuildContext context) {
    if (!AuthProvider.isLoggedIn) {
      return const MasterScreen(child: Center(child: NotLoggedInView()));
    }
    if (_isLoading) {
      return MasterScreen(
        child: const Center(child: CircularProgressIndicator()),
      );
    }
    return MasterScreen(child: _buildResultView());
  }

  Future _fetchNotifications() async {
    setState(() => _isLoading = true);
    try {
      final notifications = await _notificationsProvider.getPaged(
        page: _currentPage,
      );
      if (!mounted) return;
      setState(() => _notifications = notifications);
    } catch (ex) {
      if (!mounted) return;
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (!mounted) return;
        Helpers.showErrorMessage(context, ex);
      });
    } finally {
      if (!mounted) return;
      setState(() => _isLoading = false);
    }
  }

  Future _showDeleteDialog(BuildContext context, int id) async {
    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text("Delete"),
        content: const Text(
          "Are you sure you want to delete this notification?",
        ),
        actions: [
          TextButton(
            onPressed: () async {
              Navigator.of(ctx).pop(true);
              await _notificationsProvider.delete(id);
              await _fetchNotifications();
            },
            child: const Text("Delete"),
          ),
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(false),
            child: const Text("Cancel"),
          ),
        ],
      ),
    );
  }

  Widget _buildResultView() {
    final items = _notifications?.resultList ?? [];
    return ListView.builder(
      itemCount: items.length,
      itemBuilder: (context, index) {
        final notification = items[index];
        final isLast = index == items.length - 1;
        return Column(
          children: [
            ListTile(
              title: Text(notification.message ?? ""),
              trailing:
                  notification.bookId != null ||
                      notification.publisherId != null
                  ? Icon(Icons.chevron_right_rounded)
                  : null,
              onTap: () async {
                await _notificationsProvider.markAsRead(
                  notification.notificationId!,
                );
                //navigate to the othet screen
              },
              onLongPress: () async {
                await _showDeleteDialog(context, notification.notificationId!);
              },
            ),
            const Divider(height: 1),
            if (isLast) const Divider(height: 0.1),
          ],
        );
      },
    );
  }
}
