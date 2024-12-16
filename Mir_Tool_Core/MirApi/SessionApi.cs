namespace Mir_Utilities;

public class SessionApi
{
     public static async Task<List<SessionApiSchema.GetSessionSnapshot>?> GetSessionSnapshot(ApiCaller caller)
     {
          dynamic sessionList = await caller.GetApi("sessions");
          if (sessionList.Count == 0)
          {
               return null;
          }

          List<SessionApiSchema.GetSessionSnapshot> sessionListSnapshot = new List<SessionApiSchema.GetSessionSnapshot>();
          foreach (var session in sessionList)
          {
               SessionApiSchema.GetSessionSnapshot sessionSnapshot = new SessionApiSchema.GetSessionSnapshot();
               sessionSnapshot.Name = (session.name != null) ? session.name : "Unnamed Session";
               sessionSnapshot.Guid = (session.guid != null) ? session.guid : "Unknown Guid";
               sessionListSnapshot.Add(sessionSnapshot);
          }

          return sessionListSnapshot;
     }
     public static async Task<SessionApiSchema.GetSessionByGuidSnapshot> GetSessionByGuid(ApiCaller caller, String guid)
     {
          dynamic session = await caller.GetApi($"sessions/{guid}");
          SessionApiSchema.GetSessionByGuidSnapshot sessionSnapshot = new SessionApiSchema.GetSessionByGuidSnapshot();
          sessionSnapshot.Guid = session.guid!;
          sessionSnapshot.Name = session.name!;
          sessionSnapshot.Description = session.description!;
          return sessionSnapshot;
     }
     
     public static async Task<String> PostSession(ApiCaller caller, String guid, String name, String description)
     {
          dynamic session = new
          {
               guid,
               name,
               description
          };
          dynamic response = await caller.PostApi("sessions", session);
          return response.guid!;
     }
     
     public static async Task<String> PutSession(ApiCaller caller, String guid, String name, String description)
     {
          dynamic session = new
          {
               name,
               description
          };
          dynamic response = await caller.PutApi($"sessions/{guid}", session);
          return response.guid!;
     }
     public void DeleteSession(ApiCaller caller, String guid)
     { 
          caller.DeleteApi($"sessions/{guid}");
     }
     
     public static async Task<String> SessionExport(ApiCaller caller, String guid)
     {
          String response = await caller.GetApi($"sessions/{guid}/export");
          return response;
     }

     public static void SessionImport(ApiCaller caller, String file)
     {
          dynamic session = new { file };
          caller.PostApi("sessions/import", session);
     }
}