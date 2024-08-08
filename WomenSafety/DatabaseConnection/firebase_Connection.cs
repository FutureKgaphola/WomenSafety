
using Android.App;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

namespace WomenSafety.DatabaseConnection
{
    class firebase_Connection
    {
        public static FirebaseDatabase GetDatabase()
        {
            FirebaseDatabase database;
            var app = FirebaseApp.InitializeApp(Application.Context);
            if (app == null)
            {
                var option = new FirebaseOptions.Builder()
                    .SetApiKey("AIzaSyDcZAsOAFqlORxMfCNEuXpXlw0WNwI0nno")
                    .SetApplicationId("metrorail-5b955")
                    .SetDatabaseUrl("https://metrorail-5b955-default-rtdb.firebaseio.com")
                    .SetProjectId("metrorail-5b955")
                    .SetStorageBucket("metrorail-5b955.appspot.com")
                    .Build();
                app = FirebaseApp.InitializeApp(Application.Context, option);
                database = FirebaseDatabase.GetInstance(app);
                return database;

            }
            else
            {
                database = FirebaseDatabase.GetInstance(app);
                return database;

            }
        }
        public FirebaseAuth GetFirebaseAuth()
        {
            FirebaseAuth firebase;
            var app = FirebaseApp.InitializeApp(Application.Context);
            if (app == null)
            {
                var option = new FirebaseOptions.Builder()
                    .SetApiKey("AIzaSyDcZAsOAFqlORxMfCNEuXpXlw0WNwI0nno")
                    .SetApplicationId("metrorail-5b955")
                    .SetDatabaseUrl("https://metrorail-5b955-default-rtdb.firebaseio.com")
                    .SetProjectId("metrorail-5b955")
                    .SetStorageBucket("metrorail-5b955.appspot.com")
                    .Build();
                app = FirebaseApp.InitializeApp(Application.Context, option);
                firebase = FirebaseAuth.GetInstance(app);
                return firebase;

            }
            else
            {

                firebase = FirebaseAuth.GetInstance(app);
                return firebase;
            }

        }

    }
}