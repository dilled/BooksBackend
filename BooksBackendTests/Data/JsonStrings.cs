namespace BooksBackendTests.Data
{
    public static class JsonStrings
    {
        public static string[] validBooks = 
        {
            "{" +
                "\"title\":\"Harry Potter and the Philosophers Stone\"," +
                "\"author\":\"J.K.Rowling\"," +
                "\"year\":1997," +
                "\"publisher\":\"Bloomsbury (UK)\"," +
                "\"description\":\"A book about a wizard boy\"" +
            "}",
            "{" +
                "\"title\":\"Old Testament\"," +
                "\"author\":\"Various\"," +
                "\"year\":-165," +
                "\"publisher\":null," +
                "\"description\":\"A holy book of Christianity and Jewish faith\"" +
            "}",
            "{" +
                "\"title\":\"The Subtle Knife\"," +
                "\"author\":\"Philip Pullman\"," +
                "\"year\":1997," +
                "\"publisher\":\"Scholastic Point\"," +
                "\"description\":null" +
            "}",
            "{" +
                "\"title\":\"Goosebumps: Beware, the Snowman\"," +
                "\"author\":\"R.L. Stine\"," +
                "\"year\":1997," +
                "\"publisher\":\"Scholastic Point\"," +
                "\"description\":null" +
                
            "}"
        };

        public static string[] invalidBooks =
        {
            "{" +
                "\"author\":\"Douglas Adams\"," +
                "\"year\":1979," +
                "\"publisher\":\"Pan Books\"," +
                "\"description\":\"Originally a radio series\"" +
            "}",
            "{" +
                "\"title\":\"The Hitchhiker's Guide to the Galaxy\"," +
                "\"author\":\"Douglas Adams\"," +
                "\"publisher\":\"Pan Books\"," +
                "\"description\":\"Originally a radio series\"" +
            "}",
            "{" +
                "\"author\":\"Douglas Adams\"," +
                "\"title\":\"The Hitchhiker's Guide to the Galaxy\"," +
                "\"pages\":208," +
                "\"description\":\"Originally a radio series\"" +
            "}",
            "{" +
                "\"title\":\"The Hitchhiker's Guide to the Galaxy\"," +
                "\"author\":\"\"," +
                "\"year\":1979," +
                "\"publisher\":\"Pan Books\"," +
                "\"description\":\"Originally a radio series\"" +
            "}",
            "{" +
                "\"title\":\"\"," +
                "\"author\":\"Douglas Adams\"," +
                "\"year\":1979," +
                "\"publisher\":\"Pan Books\"," +
                "\"description\":\"Originally a radio series\"" +
            "}",
            "{" +
                "\"title\":\"The Hitchhiker's Guide to the Galaxy\"," +
                "\"author\":\"Douglas Adams\"," +
                "\"year\":1979.999," +
                "\"publisher\":\"Pan Books\"," +
                "\"description\":\"Originally a radio series\"" +
            "}",
            "{" +
                "\"title\":\"The Hitchhiker's Guide to the Galaxy\"," +
                "\"author\":\"Douglas Adams\"," +
                "\"year\":\"nineteen-ninety-seven\"," +
                "\"publisher\":\"Pan Books\"," +
                "\"description\":\"Originally a radio series\"" +
            "}",
            "{" +
                "\"title\":\"title\"," +
                "\"author\":\"Douglas Adams\"," +
                "\"year\":1979," +
                "\"publisher\":\"\"," +
                "\"description\":\"Originally a radio series\"" +
            "}"
        };

        public static string missingTitle = 
            "{" +
                "\"author\":\"Douglas Adams\"," +
                "\"year\":1979," +
                "\"publisher\":\"Pan Books\"," +
                "\"description\":\"Originally a radio series\"" +
            "}";

        public static string missingYear = 
            "{" +
                "\"title\":\"The Hitchhiker's Guide to the Galaxy\"," +
                "\"author\":\"Douglas Adams\"," +
                "\"publisher\":\"Pan Books\"," +
                "\"description\":\"Originally a radio series\"" +
            "}";

        public static string invalidYear = 
            "{" +
                "\"author\":\"Douglas Adams\"," +
                "\"title\":\"The Hitchhiker's Guide to the Galaxy\"," +
                "\"pages\":208," +
                "\"description\":\"Originally a radio series\"" +
            "}";

        public static string emptyAuthor = 
            "{" +
                "\"title\":\"The Hitchhiker's Guide to the Galaxy\"," +
                "\"author\":\"\"," +
                "\"year\":1979," +
                "\"publisher\":\"Pan Books\"," +
                "\"description\":\"Originally a radio series\"" +
            "}";

        public static string emptyTitle = 
            "{" +
                "\"title\":\"\"," +
                "\"author\":\"Douglas Adams\"," +
                "\"year\":1979," +
                "\"publisher\":\"Pan Books\"," +
                "\"description\":\"Originally a radio series\"" +
            "}";

        public static string nonIntYear = 
            "{" +
                "\"title\":\"The Hitchhiker's Guide to the Galaxy\"," +
                "\"author\":\"Douglas Adams\"," +
                "\"year\":1979.999," +
                "\"publisher\":\"Pan Books\"," +
                "\"description\":\"Originally a radio series\"" +
            "}";

        public static string stringYear = 
            "{" +
                "\"title\":\"The Hitchhiker's Guide to the Galaxy\"," +
                "\"author\":\"Douglas Adams\"," +
                "\"year\":\"nineteen-ninety-seven\"," +
                "\"publisher\":\"Pan Books\"," +
                "\"description\":\"Originally a radio series\"" +
            "}";

        public static string emptyPublisher = 
            "{" +
                "\"title\":\"title\"," +
                "\"author\":\"Douglas Adams\"," +
                "\"year\":1979," +
                "\"publisher\":\"\"," +
                "\"description\":\"Originally a radio series\"" +
            "}";
    }
}