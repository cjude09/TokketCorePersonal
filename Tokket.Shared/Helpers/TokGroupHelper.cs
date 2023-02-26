using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Core;

namespace Tokket.Shared.Helpers
{
    /// <summary>Used to get all tok groups and tok types.</summary>
    public static class TokGroupHelper
    {
        public static List<TokTypeList> TokGroups { get; set; } = new List<TokTypeList>()
        {
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Mega",
                PrimaryFieldName = "Name",
                SecondaryFieldName = "Title",
                IsDetailBased = true,
                PrimaryCharLimit = 200,
                SecondaryCharLimit = 150000,
                TokTypes = new string[]
                {
                    "Blog",
                    "Book",
                    "Chapter",
                    "Journal",
                    "Information",
                    "Letter/Email",
                    "Literature",
                    "News",
                    "Notes",
                    "Report",
                    "Sermon",
                    "Speech",
                    "Other"
                },
                TokTypeIds = new string[]
                {
                    "toktype-mega-blog",
                    "toktype-mega-book",
                    "toktype-mega-chapter",
                    "toktype-mega-journal",
                    "toktype-mega-information",
                    "toktype-mega-letteremail",
                    "toktype-mega-literature",
                    "toktype-mega-news",
                    "toktype-mega-notes",
                    "toktype-mega-report",
                    "toktype-mega-sermon",
                    "toktype-mega-speech",
                    "toktype-mega-other"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Sentence Example",
                    "Author/Attributed to",
                    "Time Period",
                    "Country/Region/Area",
                    "Pronunciation"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    200,
                    50,
                    50,
                    100,
                    600
                },
                RequiredFields = new string[]
                {

                },
                Description = "Mega toks are a section based tok format for large amounts of text.",
                Descriptions = new string[]
                {
                    "Content from a blog",
                    "Content coming from a book",
                    "Content from a chapter of a book",
                    "Content that would be put on a journal",
                    "Content from an information text",
                    "Content from a letter or email",
                    "Any literary work",
                    "Content from the news",
                    "Content taken from a lecture or presentation",
                    "Content that would be on a report",
                    "Content from a religious sermon​",
                    "Content from a speech​",
                    "Large Volume of Text e.g. Laws, Constitution, etc.​"
                },
                Example = "",
                Examples = new string[]
                {
                    "Huffington Post Blog",
                    "Huckleberry Finn",
                    "The Hunger Games Chapter 1​",
                    "The Reagan Diaries by Ronald Reagan​",
                    "Animal information from an encyclopedia​",
                    "Letter from Abraham Lincoln",
                    "Odes and Epodes​",
                    "Hurricane Katrina Disaster​",
                    "Steve Jobs iPhone introduction​",
                    "Alexander Hamilton's famous report on manufactures made to Congress 1791​",
                    "Open House For All Comers by Charles Spurgeon",
                    "Martin Luther King's 'I Have a Dream' speech",
                    "United States Constitution"
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-mega",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1568398628,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Question",
                PrimaryFieldName = "Question",
                SecondaryFieldName = "Answer",
                IsDetailBased = false,
                PrimaryCharLimit = 200,
                SecondaryCharLimit = 200,
                TokTypes = new string[]
                {
                    "Riddle",
                    "Missing Word",
                    "Who Said",
                    "Trivia",
                    "What am I?",
                    "Where am I?",
                    "Who am I?",
                    "Exam/Test",
                    "Conversation",
                    "Fact",
                    "Other Question"
                },
                TokTypeIds = new string[]
                {
                    "toktype-question-riddle",
                    "toktype-question-missingword",
                    "toktype-question-whosaid",
                    "toktype-question-trivia",
                    "toktype-question-whatami",
                    "toktype-question-whereami",
                    "toktype-question-whoami",
                    "toktype-question-examtest",
                    "toktype-question-conversation",
                    "toktype-question-fact",
                    "toktype-question-otherquestion"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Time Period",
                    "Country/Region/Area"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    50,
                    100
                },
                RequiredFields = new string[]
                {

                },
                Description = "Use this tok group for conversational, test and other questions.",
                Descriptions = new string[]
                {
                    "Puzzle-like questions that need to be solved",
                    "Any question with a blank to fill in the word(s)",
                    "Questions that involve finding out who said a specific statement",
                    "Questions with answers pertaining to a variety of topics and facts",
                    "Questions that involve trying to guess a living or non-living thing",
                    "Questions about places and can include directions on how to get to those places",
                    "Questions on guessing a person based on specific details about them",
                    "Questions similar to or directly from tests or exams",
                    "Questions normally used when someone is talking to another person",
                    "Questions that have answers proven to be true",
                    "Any question that does not fall into the other tok types"
                },
                Example = "",
                Examples = new string[]
                {
                    "Q: What is black, white and red all over? A: A newspaper",
                    "Q: Subway: Eat ___. What is the missing word? A: Fresh",
                    "Q: Who said \"Simplicity is the Key to Brilliance\".  A: Bruce Lee",
                    "Q: Who played Batman in the movie \"Justice League\"(2017).  A: Ben Affleck",
                    "Q: I am shaped like a sphere and can bounce off of any surface--What am I? A: A Ball",
                    "Q: I start in Phoenix, Arizona. I drive 372 miles West via Interstate 10--Where am I? A: Los Angeles",
                    "Q: I was the 3rd President of the United States of America and wrote the Declaration of Independence--who am I? A: Thomas Jefferson",
                    "Q: What is the main idea of this passage? A: That Tyler is struggling with school and wants to get his grades up.",
                    "Q: How are you doing today? A:I am doing fine.",
                    "Q: What is the Capital of Arizona?​ A: Phoenix",
                    "Q: Why is my stomach hurting? A: Because you ate too fast."
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-question",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1563592678,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Event",
                PrimaryFieldName = "Name",
                SecondaryFieldName = "Detail",
                IsDetailBased = true,
                PrimaryCharLimit = 200,
                SecondaryCharLimit = 200,
                TokTypes = new string[]
                {
                    "Calendar",
                    "Entertainment",
                    "Fair/Festival",
                    "Government/Political​",
                    "Historical",
                    "Holiday",
                    "Nature/Science",
                    "Religious",
                    "Sporting",
                    "Other Event​"
                },
                TokTypeIds = new string[]
                {
                    "toktype-event-calendar",
                    "toktype-event-entertainment",
                    "toktype-event-fairfestival",
                    "toktype-event-governmentpolitical",
                    "toktype-event-historical",
                    "toktype-event-holiday",
                    "toktype-event-naturescience",
                    "toktype-event-religious",
                    "toktype-event-sporting",
                    "toktype-event-otherevent"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Sentence Example",
                    "Author/Attributed to",
                    "Time Period",
                    "Country/Region/Area",
                    "Pronunciation"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    200,
                    50,
                    50,
                    100,
                    600
                },
                RequiredFields = new string[]
                {

                },
                Description = "This tok group has event related toks",
                Descriptions = new string[]
                {
                    "Event that happens on a specific date",
                    "Event that entertains others",
                    "Events that celebrate something or entertains",
                    "Event that is related to a government",
                    "Event that is important in history",
                    "Event that happens in a holiday",
                    "Event that occurs in nature or science",
                    "Event related to religion​",
                    "Event that involves sports or athletes",
                    "Events of business, retail, future, etc.​"
                },
                Example = "The beginning of Tokkepedia",
                Examples = new string[]
                {
                    "Leap Year",
                    "Coachella",
                    "Mardi Gras​",
                    "2020 United States presidential election",
                    "November 19, 1918 - WWI ends​",
                    "Macy's Thanksgiving Parade -Thanksgiving Day",
                    "Solar Eclipse",
                    "Easter Sunday",
                    "Super Bowl​",
                    "Black Friday"
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-event",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1563591079,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Quote",
                PrimaryFieldName = "Quote",
                SecondaryFieldName = "Source",
                IsDetailBased = false,
                PrimaryCharLimit = 600,
                SecondaryCharLimit = 200,
                TokTypes = new string[]
                {
                    "Movie",
                    "Book",
                    "Lyric",
                    "Poem",
                    "Speech",
                    "Social Media",
                    "Article",
                    "Person",
                    "News",
                    "Prayer",
                    "Character",
                    "Scripture",
                    "Motto/Slogan",
                    "Organization"
                },
                TokTypeIds = new string[]
                {
                    "toktype-quote-movie",
                    "toktype-quote-book",
                    "toktype-quote-lyric",
                    "toktype-quote-poem",
                    "toktype-quote-speech",
                    "toktype-quote-socialmedia",
                    "toktype-quote-article",
                    "toktype-quote-person",
                    "toktype-quote-news",
                    "toktype-quote-prayer",
                    "toktype-quote-character",
                    "toktype-quote-scripture",
                    "toktype-quote-mottoslogan",
                    "toktype-quote-organization"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Author/Attributed to",
                    "Time Period",
                    "Country/Region/Area"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    50,
                    50,
                    100
                },
                RequiredFields = new string[]
                {

                },
                Description = "Use this group for sayings and statements that can be attributed to a person or written document",
                Descriptions = new string[]
                {
                    "Something said from a movie",
                    "Something said in any book",
                    "Lines in any song",
                    "Lines from any poem",
                    "Lines from any speech",
                    "Words from a post off of any social media app",
                    "Lines from web articles, newspapers, magazines, etc.",
                    "Any original statement said by a person",
                    "Any statement from a news anchor or news source",
                    "Any lines from a prayer",
                    "Something said from a character in any movie, game, etc.",
                    "Lines from any religious scripture",
                    "A 'motto' is a phrase that expresses the principle guiding the behavior of a particular group.  A 'slogan' is a phrase that is easy to remember and used by a group to attract attention",
                    "Any statement from an organization"
                },
                Example = "",
                Examples = new string[]
                {
                    "\"Why so serious?\"-The Dark Knight",
                    "\"It's the possibility of having a dream come true that makes life interesting\"-The Alchemist",
                    "\"Amazing Grace! How sweet the sound\"",
                    "\"Tis better to have loved and lost than never to have loved at all\"- In Memoriam A.H.H.",
                    "\"I have a dream that my four children will one day live in a nation where they will not be judged by the color of their skin but by the content of their character.\"-I have a dream speech by Martin Luther King Jr.",
                    "\"Our two great republics are linked together by the timeless bonds of history, culture, and destiny. We are people who cherish our values, protect our civilization, and recognize the image of God in every human soul.\" Donald J. Trump @realDonaldTrump  4/24/18",
                    "\"Ferguson, a city of 21,000, is unusual in some respects — it has issued the most warrants of any city in the state relative to its size\"-The New York Times",
                    "\"The true sign of intelligence is not knowledge, but imagination\"-Albert Einstein",
                    "\"The attempt to invoke his testimony was offbase\"-Ari Melber of MSNBC",
                    "\"Our father who art in heaven...\"-The Lord's Prayer",
                    "\"Why so serious?\"-The Joker",
                    "\"Trust in the Lord with all thine heart; and lean not unto thine own understanding.\"-Proverbs 3:5",
                    "1) U.S. Marines' motto: Semper Fidelis, Always Faithful 2) KFC slogan\" “Finger Lickin' Good”",
                    "Don't be evil - Google"
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-quote",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1557158153,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Test",
                PrimaryFieldName = "Name",
                SecondaryFieldName = "Detail",
                IsDetailBased = true,
                PrimaryCharLimit = 200,
                SecondaryCharLimit = 200,
                TokTypes = new string[]
                {
                    "Certification",
                    "License",
                    "Subject",
                    "Standardized",
                    "Elementary",
                    "Junior High",
                    "High School",
                    "College",
                    "Other Test"
                },
                TokTypeIds = new string[]
                {
                    "toktype-test-certification",
                    "toktype-test-license",
                    "toktype-test-subject",
                    "toktype-test-standardized",
                    "toktype-test-elementary",
                    "toktype-test-juniorhigh",
                    "toktype-test-highschool",
                    "toktype-test-college",
                    "toktype-test-othertest"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Sentence Example",
                    "Author/Attributed to",
                    "Time Period",
                    "Country/Region/Area",
                    "Pronunciation"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    200,
                    50,
                    50,
                    100,
                    600
                },
                RequiredFields = new string[]
                {

                },
                Description = "Use this group for content related to school or professional exams.",
                Descriptions = new string[]
                {
                    "Content from any certification test",
                    "Content from tests needed to get any license",
                    "Content from tests of a particular subject",
                    "Content from standardized tests",
                    "Content from any elementary school given test",
                    "Content from any junior high school given test",
                    "Content from any high school given test",
                    "Content from any college test",
                    "Content from any other test"
                },
                Example = "",
                Examples = new string[]
                {
                    "NCLEX Exam=standardized exam that each state board of nursing uses to determine whether or not a candidate is prepared for entry-level nursing practice.",
                    "boating license test",
                    "Math test, English test",
                    "ACT, SAT",
                    "3rd Grade Spelling Test",
                    "Pre-Algebra Lesson 5 test",
                    "English 10 vocab test",
                    "Psychology Test",
                    "DUI test"
                },
                HasAnswerField = true,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-test",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1551072291,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Word",
                PrimaryFieldName = "Word(s)",
                SecondaryFieldName = "Definition",
                IsDetailBased = false,
                PrimaryCharLimit = 200,
                SecondaryCharLimit = 600,
                TokTypes = new string[]
                {
                    "Definition",
                    "Crossword",
                    "Allusion"
                },
                TokTypeIds = new string[]
                {
                    "toktype-word-definition",
                    "toktype-word-crossword",
                    "toktype-word-allusion"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Sentence Example",
                    "Time Period",
                    "Country/Region/Area",
                    "Pronunciation"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    200,
                    50,
                    100,
                    600
                },
                RequiredFields = new string[]
                {

                },
                Description = "Use this for crosswords or word definitons",
                Descriptions = new string[]
                {
                    "The meaning(s) of a single word",
                    "Words that would go in a crossword",
                    "An expression that makes an indirect reference to something : the act of alluding to something"
                },
                Example = "",
                Examples = new string[]
                {
                    "Array: An ordered series of arrangements",
                    "Clue - Beginning, Word - Outset",
                    "1) Achilles heel - weakness, vulnerable point.  2) Good Samaritan - one who selflessly helped another person, especially a stranger."
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-word",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1548827729,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Rule/Principle",
                PrimaryFieldName = "Name",
                SecondaryFieldName = "Detail",
                IsDetailBased = false,
                PrimaryCharLimit = 600,
                SecondaryCharLimit = 200,
                TokTypes = new string[]
                {
                    "Mission",
                    "Value",
                    "Vision",
                    "Policy",
                    "Practice",
                    "Strategy",
                    "Law",
                    "Rule",
                    "Other Rule/Principle"
                },
                TokTypeIds = new string[]
                {
                    "toktype-ruleprinciple-mission",
                    "toktype-ruleprinciple-value",
                    "toktype-ruleprinciple-vision",
                    "toktype-ruleprinciple-policy",
                    "toktype-ruleprinciple-practice",
                    "toktype-ruleprinciple-strategy",
                    "toktype-ruleprinciple-law",
                    "toktype-ruleprinciple-rule",
                    "toktype-ruleprinciple-otherruleprinciple"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Sentence Example",
                    "Author/Attributed to",
                    "Time Period",
                    "Country/Region/Area",
                    "Pronunciation"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    200,
                    50,
                    50,
                    100,
                    600
                },
                RequiredFields = new string[]
                {

                },
                Description = "Use this for content such as legal laws, mission statements, values, etc.",
                Descriptions = new string[]
                {
                    "Any mission statement given by any group or organization",
                    "Any value",
                    "Any vision",
                    "Policy from any place",
                    "Any practice",
                    "int term approach to accomplish a int term objective",
                    "Laws or principles from government, science, business, etc.",
                    "A regulation, stipulation, or instruction to follow.",
                    "Any other rule/principle that does not fit the current tok types"
                },
                Example = "",
                Examples = new string[]
                {
                    "Starbucks Mission-“To inspire and nurture the human spirit – one person, one cup and one neighborhood at a time.”",
                    "Values of Apple: innovation, quality, team spirit",
                    "McDonald's Vision-\"To be the best quick service restaurant experience\". ",
                    "Return policy: You can return any item within 30 days if you have a receipt",
                    "Business practice example: partner with employees",
                    "Attrition: A strategy which seeks to gradually erode the combat power of the enemy's armed forces",
                    "2nd Amendment: The right to bear arms",
                    "At a public swimming pool: no running, no diving etc.",
                    "Adjust your atitude frequently"
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-ruleprinciple",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1548827729,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Subjective",
                PrimaryFieldName = "Statement",
                SecondaryFieldName = "Point",
                IsDetailBased = true,
                PrimaryCharLimit = 200,
                SecondaryCharLimit = 200,
                TokTypes = new string[]
                {
                    "Recommendation",
                    "Review",
                    "Opinion/Statement",
                    "Best",
                    "Worst",
                    "Prediction"
                },
                TokTypeIds = new string[]
                {
                    "toktype-subjective-recommendation",
                    "toktype-subjective-review",
                    "toktype-subjective-opinionstatement",
                    "toktype-subjective-best",
                    "toktype-subjective-worst",
                    "toktype-subjective-prediction"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Sentence Example",
                    "Author/Attributed to",
                    "Time Period",
                    "Country/Region/Area",
                    "Pronunciation"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    200,
                    50,
                    50,
                    100,
                    600
                },
                RequiredFields = new string[]
                {

                },
                Description = "This tok group is for peoples opinions like reccomendations, reviews, etc.",
                Descriptions = new string[]
                {
                    "Anything you can recommend to another person",
                    "Assessment of something",
                    "How you feel about something",
                    "The best things in any category",
                    "The worst things in any category",
                    "A forecast that something is going to happen or not going to happen in the future"
                },
                Example = "---",
                Examples = new string[]
                {
                    "Reccomendation to buy a Subaru for its safety rating",
                    "The Incredibles 2 is an undeniable triumph, but it is also so keenly aware that it becomes exhausting. -The New Republic",
                    "I like Carribean beaches because of 1.The white sand 2. The warm water 3. The clear blue water",
                    "Best Places to retire according to Money Magazine",
                    "Worst safety rated cars according to Consumer Reports",
                    "The dissolution of the USSR was predicted by Emmanuel Todd on the basis of economic and national factors."
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-subjective",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1548827728,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Saying",
                PrimaryFieldName = "Saying",
                SecondaryFieldName = "Meaning",
                IsDetailBased = false,
                PrimaryCharLimit = 600,
                SecondaryCharLimit = 200,
                TokTypes = new string[]
                {
                    "Slang",
                    "Idiom",
                    "Euphemism",
                    "Cliché",
                    "Metaphor",
                    "Religious",
                    "Language",
                    "Regional",
                    "Latin",
                    "Proverbs",
                    "Other Saying"
                },
                TokTypeIds = new string[]
                {
                    "toktype-saying-slang",
                    "toktype-saying-idiom",
                    "toktype-saying-euphemism",
                    "toktype-saying-cliche",
                    "toktype-saying-metaphor",
                    "toktype-saying-religious",
                    "toktype-saying-language",
                    "toktype-saying-regional",
                    "toktype-saying-latin",
                    "toktype-saying-proverbs",
                    "toktype-saying-othersaying"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Sentence Example",
                    "Author/Attributed to",
                    "Time Period",
                    "Country/Region/Area",
                    "Pronunciation"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    200,
                    50,
                    50,
                    100,
                    600
                },
                RequiredFields = new string[]
                {

                },
                Description = "Use this group for phrases that are not quoted from someone or a written document",
                Descriptions = new string[]
                {
                    "An informal word or phrase started by a group of people",
                    "A figurative way of saying something with a different literal meaning",
                    "A mild or indirect word or expression substituted for one considered to be too harsh or blunt when referring to something unpleasant or embarrassing",
                    "A phrase that is overused that now lacks originality and has become bad writing or old fashioned",
                    "A figure of speech containing an implied comparison, in which a word or phrase ordinarily and primarily used of one thing is applied to another",
                    "Phrases spoken in your religious group",
                    "Phrases in a foreign language used by other countries",
                    "An expression that is said locally or within a given region",
                    "Sayings in the Latin langauge",
                    "wisdom sayings",
                    "Use if you don't know which Tok type to select for the saying"
                },
                Example = "",
                Examples = new string[]
                {
                    "1) Far out - means cool  2) Can You Dig It - means do you understand?",
                    "1) under the weather = sick or not feeling well 2)  chip on your shoulder = being  upset for something that happened in the past. ",
                    "1) downsizing - means terminating a group of employees 2) put to sleep -  means euthanize or kill",
                    "1) what goes around comes around 2) a needle in the haystack",
                    "1) America is a melting pot 2) He is a night owl.",
                    "1) God is great 2) Where God guides, God provides.",
                    "1) Hasta la vista is a Spanish farewell and means see you later 2) bon voyage is a French saying meaning have a nice trip",
                    "1) U.S. South: y'all = you all, two or more people 2) U.K. North: where there's muck, there's brass =  good money can be made from dirty jobs.",
                    "1) carpe diem - seize the day 2) pro bono abbreviated from Latin pro bono publico (“for the public good”) - without fee",
                    "1) the early bird gets the worm -means the one who arrives first has the best chance for success 2) 'a sparrow in thy hand is better than a thousand sparrows flying' -means It is preferable to have a small but certain advantage than a mere potential of a greater one",
                    "1) As iron sharpens iron, so one man sharpens another (proverb, metaphor and other) 2) nitty-gritty  (slang or other)"
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-saying",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1548827728,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Title",
                PrimaryFieldName = "Title",
                SecondaryFieldName = "Description",
                IsDetailBased = false,
                PrimaryCharLimit = 200,
                SecondaryCharLimit = 600,
                TokTypes = new string[]
                {
                    "Play",
                    "Musical",
                    "Opera",
                    "Book",
                    "Song",
                    "Magazine",
                    "Movies",
                    "Game",
                    "TV Show",
                    "Event",
                    "Article",
                    "Other Title"
                },
                TokTypeIds = new string[]
                {
                    "toktype-title-play",
                    "toktype-title-musical",
                    "toktype-title-opera",
                    "toktype-title-book",
                    "toktype-title-song",
                    "toktype-title-magazine",
                    "toktype-title-movies",
                    "toktype-title-game",
                    "toktype-title-tvshow",
                    "toktype-title-event",
                    "toktype-title-article",
                    "toktype-title-othertitle"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Author/Attributed to",
                    "Time Period",
                    "Country/Region/Area"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    50,
                    50,
                    100
                },
                RequiredFields = new string[]
                {

                },
                Description = "Use this for book titles, movie titles, and other titles",
                Descriptions = new string[]
                {
                    "Title for any play in any given form(book, movie, live perforance, etc.)",
                    "Title of any musical in any given form",
                    "Title of any opera",
                    "Title of any book",
                    "Title for any song",
                    "Title of any magazine",
                    "Title of a Film released in theaters or on dvd",
                    "Title of any board or video game",
                    "Title for any TV Show or Netflix Series",
                    "Title of any historical, annual, sports, or any other event",
                    "Title of any news or publication article",
                    "Any title that does not fall under the other Tok Types for Titles"
                },
                Example = "",
                Examples = new string[]
                {
                    "Romeo and Juliet",
                    "The Greatest Showman",
                    "Carmen",
                    "The Hobbit",
                    "Billie Jean",
                    "Time Magazine",
                    "The Avengers",
                    "Fortnite",
                    "Game of Thrones",
                    "Coachella",
                    "\"Trump orders migrant families to be detained together\"-Wall Street Journal",
                    "Duchess of Sussex-Royalty title"
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-title",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1548827728,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Organization",
                PrimaryFieldName = "Name",
                SecondaryFieldName = "Detail",
                IsDetailBased = true,
                PrimaryCharLimit = 200,
                SecondaryCharLimit = 200,
                TokTypes = new string[]
                {
                    "Club ",
                    "Religious Organization",
                    "Educational Organization",
                    "Charitable Organization",
                    "Scientific Organization",
                    "Nonprofit",
                    "Government",
                    "Military",
                    "Small Business",
                    "Corporation",
                    "Company",
                    "Other Organization"
                },
                TokTypeIds = new string[]
                {
                    "toktype-organization-club",
                    "toktype-organization-religiousorganization",
                    "toktype-organization-educationalorganization",
                    "toktype-organization-charitableorganization",
                    "toktype-organization-scientificorganization",
                    "toktype-organization-nonprofit",
                    "toktype-organization-government",
                    "toktype-organization-military",
                    "toktype-organization-smallbusiness",
                    "toktype-organization-corporation",
                    "toktype-organization-company",
                    "toktype-organization-otherorganization"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Sentence Example",
                    "Author/Attributed to",
                    "Time Period",
                    "Country/Region/Area",
                    "Pronunciation"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    200,
                    50,
                    50,
                    100,
                    600
                },
                RequiredFields = new string[]
                {

                },
                Description = "Use this tok group for non-profit, profit, or club organizations.",
                Descriptions = new string[]
                {
                    "A social, school, sports, etc. organization",
                    "Anything related to a Religious Organization",
                    "A school or any other educational organization",
                    "Anything related to a charitable organization",
                    "Anything related to a scientific organization",
                    "Anything related to a nonprofit organization",
                    "Anything related to city, county, state, federal government",
                    "Anything related to the military of any country",
                    "Anything related to a Small Business",
                    "A company that is legally incorporated as a C or S corporation",
                    "Any business organization",
                    "Any other organization"
                },
                Example = "",
                Examples = new string[]
                {
                    "Honor Society: Phi Theta Kappa",
                    "Hillsong United",
                    "Harvard University",
                    "Wounded Warrior Project",
                    "NASA",
                    "The Salvation Army",
                    "U.S. Legislative Branch",
                    "U.S. Marine Corps",
                    "Dutch Bros Coffee",
                    "Microsoft Corporation",
                    "Apple",
                    "Alcoholic Anonymous"
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-organization",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1548827728,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Thing",
                PrimaryFieldName = "Name",
                SecondaryFieldName = "Detail",
                IsDetailBased = true,
                PrimaryCharLimit = 200,
                SecondaryCharLimit = 200,
                TokTypes = new string[]
                {
                    "Plant",
                    "Animal",
                    "Product",
                    "Service",
                    "Game",
                    "Food",
                    "Drink",
                    "Other Thing"
                },
                TokTypeIds = new string[]
                {
                    "toktype-thing-plant",
                    "toktype-thing-animal",
                    "toktype-thing-product",
                    "toktype-thing-service",
                    "toktype-thing-game",
                    "toktype-thing-food",
                    "toktype-thing-drink",
                    "toktype-thing-otherthing"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Time Period",
                    "Country/Region/Area"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    50,
                    100
                },
                RequiredFields = new string[]
                {

                },
                Description = "Products, consumables,services, and living things.",
                Descriptions = new string[]
                {
                    "A living organism that synthesizes nutrients in its leaves by photosynthesis.",
                    "Any living thing that feeds on organic matter",
                    "Anything manufactured for sale",
                    "The action of doing something for someone",
                    "Any activity with a goal as well as rules and regulations",
                    "Anything that people eat",
                    "Any consumable liquid",
                    "Anything that does not fall under the other Tok Group"
                },
                Example = "",
                Examples = new string[]
                {
                    "Venus Flytrap",
                    "Grizzly Bear",
                    "Apple iPhone",
                    "Pest Control",
                    "Monopoly",
                    "Pizza",
                    "Coca-Cola",
                    "Black Hole"
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-thing",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1548827728,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Thesaurus",
                PrimaryFieldName = "Word",
                SecondaryFieldName = "Associated Words",
                IsDetailBased = false,
                PrimaryCharLimit = 200,
                SecondaryCharLimit = 600,
                TokTypes = new string[]
                {
                    "Synonym",
                    "Rhyme",
                    "Antonym"
                },
                TokTypeIds = new string[]
                {
                    "toktype-thesaurus-synonym",
                    "toktype-thesaurus-rhyme",
                    "toktype-thesaurus-antonym"
                },
                OptionalFields = new string[]
                {

                },
                OptionalLimits = new long[]
                {

                },
                RequiredFields = new string[]
                {

                },
                Description = "Use this group for similar, opposite, or rhyming words",
                Descriptions = new string[]
                {
                    "Words with similar meaning to a given word",
                    "Word that sounds like another word",
                    "Words with opposite meaning to a given word"
                },
                Example = "",
                Examples = new string[]
                {
                    "Word - great, Synonyms - amazing, spectacular, phenominal",
                    "Word - chair, Rhyming Words - hair, scare, bear",
                    "Word - big, Antonyms - small, tiny, micro"
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-thesaurus",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1548827728,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Story",
                PrimaryFieldName = "Title",
                SecondaryFieldName = "Summary Point",
                IsDetailBased = true,
                PrimaryCharLimit = 200,
                SecondaryCharLimit = 200,
                TokTypes = new string[]
                {
                    "Myth",
                    "Urban Legend",
                    "Miracle",
                    "Fable",
                    "Fairy Tale",
                    "Children's",
                    "Comics",
                    "Adventure",
                    "Personal",
                    "Short",
                    "Science Fiction",
                    "Romance",
                    "Horror",
                    "Mystery",
                    "Crime",
                    "Tragedy",
                    "Drama",
                    "Other Story",
                    "Folklore"
                },
                TokTypeIds = new string[]
                {
                    "toktype-story-myth",
                    "toktype-story-urbanlegend",
                    "toktype-story-miracle",
                    "toktype-story-fable",
                    "toktype-story-fairytale",
                    "toktype-story-childrens",
                    "toktype-story-comics",
                    "toktype-story-adventure",
                    "toktype-story-personal",
                    "toktype-story-short",
                    "toktype-story-sciencefiction",
                    "toktype-story-romance",
                    "toktype-story-horror",
                    "toktype-story-mystery",
                    "toktype-story-crime",
                    "toktype-story-tragedy",
                    "toktype-story-drama",
                    "toktype-story-otherstory",
                    "toktype-story-folklore"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Sentence Example",
                    "Author/Attributed to",
                    "Time Period",
                    "Country/Region/Area",
                    "Pronunciation"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    200,
                    50,
                    50,
                    100,
                    600
                },
                RequiredFields = new string[]
                {

                },
                Description = "A real or fictional story",
                Descriptions = new string[]
                {
                    "A story that is false ",
                    "Story circulated to be true but is not true",
                    "Events unexplainable by natural/physical laws",
                    "Short story with a moral",
                    "Story about magical beings and lands",
                    "Story meant for children",
                    "Story told through panels with pictures",
                    "Story involving characters going on an adventure",
                    "Story about a personal experience",
                    "Story that is not int",
                    "Story based on scientific advances or environmental change",
                    "Story involving love",
                    "Story that is meant to scare you",
                    "Story that is about something difficult to explain",
                    "Story invloving a solved crime",
                    "Story with a sad ending",
                    "Story full of exciting and unexpected events",
                    "Any story that does not fall under the other Tok Types",
                    "Story from a culture"
                },
                Example = "---",
                Examples = new string[]
                {
                    "Odyssey: 1.The poem mainly focuses on the Greek hero Odysseus (known as Ulysses in Roman myths), king of Ithaca, and his journey home after the fall of Troy. 2. It is, in part, a sequel to the Iliad, the other work ascribed to Homer. 3. Begins after the end of the ten-year Trojan War",
                    "Bunny Man: 1. Urban legend that originated from two incidents in Fairfax County, Virginia, in 1970, but has been spread throughout the Washington, D.C., area. 2. The legend has many variations; most involve a man wearing a rabbit costume who attacks people with an axe or hatchet. 3. Most of the stories occur around Colchester Overpass, a Southern Railway overpass spanning Colchester Road near Clifton, Virginia.",
                    "Coach Frank Martin's Miracle 2016:  1. He was near death due to pancreatic cancer 2. A nurse kneeled and grabbed his hand and began to pray..  3. He was healed , no inter having pancreatic cancer, and the nurse was never seen again.",
                    "The Tortoise and the Hare: 1. one of Aesop's Fables and is numbered 226 in the Perry Index. 2. It is itself a variant of a common folktale theme in which ingenuity and trickery (rather than doggedness) are employed to overcome a stronger opponent. 3.  The account of a race between unequal partners has attracted conflicting interpretations.",
                    "Beauty and the Beast: 1. Fairy tale written by French novelist Gabrielle-Suzanne Barbot de Villeneuve 2. Published in 1740 3. It was influenced by some earlier stories, such as \"Cupid and Psyche\" written by Lucius Apuleius Madaurensis.",
                    "Goodnight Moon: 1. American children's book written by Margaret Wise Brown and illustrated by Clement Hurd. 2. It was published on September 3, 1947. 3. It is a highly acclaimed bedtime story. ",
                    "Garfield: 1. Comic created by Jim Davis. 2. Published since 1978, it chronicles the life of the title character, the cat; Jon Arbuckle, the human; and Odie, the dog. 3. As of 2013, it was syndicated in roughly 2,580 newspapers and journals.",
                    "The Hobbit:1 Written by J.R.R. Tolkien 2. Also known as \"There and Back Again\" 3. Sixth best selling single-volume book of all time",
                    "Lebron James opening up the \"I Promise school\": 1. As a fourth grader, he says, he missed 83 days of school. 2. Mentors helped him attend school every day in 5th grade. 3. He opened up a school to help kids be sucessful at school.",
                    "The Tell-Tale Heart: 1. A short story by American writer Edgar Allan Poe, 2. It was first published in 1843. 3.  It is relayed by an unnamed narrator who endeavors to convince the reader of his sanity while simultaneously describing a murder he committed.",
                    "Ender's Game: 1. Series of science fiction books written by American author Orson Scott Card. 2. The series is set in a future where mankind is facing annihilation by an aggressive alien society. 3. The series protagonist is one of the child soldiers trained at Battle School to be the future leaders for the protection of Earth.",
                    "The Fault in Our Stars: 1.It is the sixth novel by author John Green. 2. The title is inspired by Act 1, Scene 2 of Shakespeare's play Julius Caesar. 3. The story is narrated by Hazel Grace Lancaster, a 16-year-old girl with cancer.",
                    "IT: 1. 1986 horror novel by American author Stephen King. 2. The story follows the experiences of seven children as they are terrorized by an entity that exploits the fears and phobias of its victims to disguise itself while hunting its prey. 3. The novel won the British Fantasy Award in 1987.",
                    "The Murders in the Rue Morgue: 1.it is a story by Edgar Allan Poe 2. It was published in Graham's Magazine in 1841. 3. It has been recognized as the first modern detective story. ",
                    "Assassination of Abraham Lincoln: 1. The 16th President of the United States was assassinated by well-known stage actor John Wilkes Booth on April 14, 1865.  2. He was the first American president to be assassinated.  3.It was part of a larger conspiracy intended by Booth to revive the Confederate cause.",
                    "Antony and Cleopatra: 1. Tragedy by William Shakespeare. 2. The play was performed first circa 1607 at the Blackfriars Theatre or the Globe Theatre by the King's Men. 3. The plot is based on Thomas North's translation of Plutarch's Lives and follows the relationship between Cleopatra and Mark Antony.",
                    "Hamlet: 1.Written by William Shakespeare at an uncertain date between 1599 and 1602. 2. Set in Denmark, the play dramatises the revenge Prince Hamlet is called to wreak upon his uncle, Claudius, by the ghost of Hamlet's father, King Hamlet. 3. It is Shakespeare's intest play.",
                    "True Grit: 1. 1968 novel by Charles Portis that was first published as a 1968 serial in The Saturday Evening Post. 2. The novel is told from the perspective of a woman named Mattie Ross, who recounts the time when she was 14 and sought retribution for the murder of her father by a scoundrel, Tom Chaney. 3. It is considered by some critics to be \"one of the great American novels.\"",
                    "Bloody Mary: 1. Folklore legend consisting of a ghost, phantom, or spirit conjured to reveal the future. 2. She is said to appear in a mirror when her name is chanted repeatedly. 3. The ritual encouraged young women to walk up a flight of stairs backward holding a candle and a hand mirror, in a darkened house."
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-story",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1548827727,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Steps",
                PrimaryFieldName = "Name",
                SecondaryFieldName = "Step",
                IsDetailBased = true,
                PrimaryCharLimit = 200,
                SecondaryCharLimit = 200,
                TokTypes = new string[]
                {
                    "Process",
                    "Technique",
                    "Recipe",
                    "Procedure",
                    "Formula",
                    "Dance",
                    "Stages/Phases",
                    "Other Steps"
                },
                TokTypeIds = new string[]
                {
                    "toktype-steps-process",
                    "toktype-steps-technique",
                    "toktype-steps-recipe",
                    "toktype-steps-procedure",
                    "toktype-steps-formula",
                    "toktype-steps-dance",
                    "toktype-steps-stagesphases",
                    "toktype-steps-othersteps"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Author/Attributed to",
                    "Time Period",
                    "Country/Region/Area"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    50,
                    50,
                    100
                },
                RequiredFields = new string[]
                {

                },
                Description = "Anything that requires several steps to complete such as dance, formulas, processes etc.",
                Descriptions = new string[]
                {
                    "Highlighting the specific steps in a small or big process",
                    "Steps used to learn or master a technique for sports, martial arts, etc.",
                    "Steps used for cooking and baking a food recipe",
                    "Steps taken in order to get a certain action done",
                    "Steps from Mathmatical or Scientific Formula",
                    "Steps taken to learn or do a dance",
                    "Small part(s) of a big process",
                    "Steps that do not fall under any of the Tok Types in the Tok Group \"Steps\""
                },
                Example = "",
                Examples = new string[]
                {
                    "Order a product online: 1. Search online for product to purchase. 2. Add product to shopping cart. 3. Check out with purchaser and shipping information. 4. Enter payment information 5. Complete purchase and receive confirmation",
                    "How to throw a jab 1. Stand in a boxing stance 2. Extend and rotate your front arm as you punch ",
                    "Pancake Steps 1. Mix the flour, sugar, baking powder and salt in a big bowl.  Make room in the center and pour in milk, egg and oil and mix them until smooth. 2. Heat a lightly oiled griddle or frying pan over medium to high heat. Pour the batter onto the griddle, and use 1/4 cup for each pancake. Brown on both sides and serve hot.",
                    "How to change a lightbulb 1. Turn off light 2. Unscrew bad lightbulb 3. Screw in new Lightbulb 4. Turn on light",
                    "Pythagorean theorem: a^2+b^2=c^2",
                    "Two-Step (forward):  1)  forward step onto the right foot 2)  a closing step with the left foot and 3) a forward step onto the right foot.",
                    "SDLC: 1)Planning 2)Defining 3)Designing 4)Building 5)Testing 6)Deployment",
                    "Measure miles traveled: 1. Write down starting odometer mileage 2. Drive to location 3. Write down ending odometer mileage 4. Subtract starting mileage from ending mileage"
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-steps",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1548827727,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Sport",
                PrimaryFieldName = "Name",
                SecondaryFieldName = "Detail",
                IsDetailBased = true,
                PrimaryCharLimit = 200,
                SecondaryCharLimit = 200,
                TokTypes = new string[]
                {
                    "Baseball",
                    "Basketball",
                    "Football (American)",
                    "Soccer (Futbol)",
                    "Hockey",
                    "Golf",
                    "Tennis",
                    "Motor Racing",
                    "Combat Sport/Martial Art",
                    "Track & Field",
                    "Winter Sport",
                    "Water Sport",
                    "Cycling",
                    "Sporting Event",
                    "Horse Racing",
                    "Other Sport"
                },
                TokTypeIds = new string[]
                {
                    "toktype-sport-baseball",
                    "toktype-sport-basketball",
                    "toktype-sport-footballamerican",
                    "toktype-sport-soccerfutbol",
                    "toktype-sport-hockey",
                    "toktype-sport-golf",
                    "toktype-sport-tennis",
                    "toktype-sport-motorracing",
                    "toktype-sport-combatsportmartialart",
                    "toktype-sport-trackandfield",
                    "toktype-sport-wintersport",
                    "toktype-sport-watersport",
                    "toktype-sport-cycling",
                    "toktype-sport-sportingevent",
                    "toktype-sport-horseracing",
                    "toktype-sport-othersport"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Sentence Example",
                    "Author/Attributed to",
                    "Time Period",
                    "Country/Region/Area",
                    "Pronunciation"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    200,
                    50,
                    50,
                    100,
                    600
                },
                RequiredFields = new string[]
                {

                },
                Description = "Use this tok group for any sport related content.",
                Descriptions = new string[]
                {
                    "Anyting related to baseball",
                    "Anything related to basketball",
                    "Anything related to American football",
                    "Anything related to soccer/futbol",
                    "Anything related to hockey",
                    "Anything related to golf",
                    "Anything related to tennis",
                    "Anything related to motor racing",
                    "Anything related to any combat sport",
                    "Anything related to track and field",
                    "Anything related to any winter sport",
                    "Anything related to any water sport",
                    "Anything related to cycling",
                    "Anything related to any sporting event",
                    "Anything related to horseracing",
                    "Anything related to any other type of sport"
                },
                Example = "",
                Examples = new string[]
                {
                    "MLB:Major League Baseball",
                    "March Madness:NCAA College National Championship Baskeball Tournament in March",
                    "New England Patriots: NFL team in Massachusetts",
                    "World Cup:FIFA Soccer/futbol tournament with qualified countries that occurs every four years.",
                    "Stanley Cup: Trophy for winning the NHL Championship",
                    "PGA: Professonal Golf Association",
                    "Grand Slam-When a tennis player wins the four major tournaments in 1 year: Australian Open, French Open, Wimbledon, US Open",
                    "NASCAR: National Association for Stock Car Auto Racing ",
                    "MMA: Mixed Martial Arts",
                    "marathon: 26.2 mile running race",
                    "Super-G:super giant slalom ski race",
                    "America's Cup: Famous race where 2 top yachts race against each other",
                    "Tour De France:Prestigious French cycling race in July",
                    "Olympic Games: Tournament of many sports where countries compete against each other every four years.",
                    "Triple Crown-Winning the following 3 horse races in the United States: Belmont Stakes, Kentucky Derby, Preakness Stakes",
                    "Curling:Sport in which players slide stones on a sheet of ice towards a target area which is segmented into four concentric circles."
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-sport",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1548827727,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Choices",
                PrimaryFieldName = "Question",
                SecondaryFieldName = "Possibility",
                IsDetailBased = true,
                PrimaryCharLimit = 200,
                SecondaryCharLimit = 200,
                TokTypes = new string[]
                {
                    "Conversation Responses",
                    "Multiple Scenarios",
                    "Other Choices"
                },
                TokTypeIds = new string[]
                {
                    "toktype-choices-conversationresponses",
                    "toktype-choices-multiplescenarios",
                    "toktype-choices-otherchoices"
                },
                OptionalFields = new string[]
                {
                    "Country/Region/Area"
                },
                OptionalLimits = new long[]
                {
                    100
                },
                RequiredFields = new string[]
                {

                },
                Description = "This tok group has toks where there are multiple options in how to respond, what to pick or what can be the outcome.",
                Descriptions = new string[]
                {
                    "Choices from conversaion responses",
                    "Choices from multiple scenarios",
                    "Does not fit under any choices tok types"
                },
                Example = "",
                Examples = new string[]
                {
                    "How are you? 1. I'm fine 2. I'm not feeling well 3. Don't worry about me, how are you?",
                    "Car ran a red light: 1. A car can hit it. 2. The car can hit another car, pedestrian or bicycle. 3. The car crosses the intersection without an incident.",
                    "I want a scoop of ice cream.  I can choose: 1. Vanilla 2. Strawberry 3. Chocolate"
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-choices",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1548827727,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Game",
                PrimaryFieldName = "Name",
                SecondaryFieldName = "Detail",
                IsDetailBased = true,
                PrimaryCharLimit = 100,
                SecondaryCharLimit = 100,
                TokTypes = new string[]
                {
                    "Mobile Game",
                    "PC Game",
                    "Console Game",
                    "Board Game",
                    "Card Game",
                    "Other Game"
                },
                TokTypeIds = new string[]
                {
                    "toktype-game-mobilegame",
                    "toktype-game-pcgame",
                    "toktype-game-consolegame",
                    "toktype-game-boardgame",
                    "toktype-game-cardgame",
                    "toktype-game-othergame"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Sentence Example",
                    "Author/Attributed to",
                    "Time Period",
                    "Country/Region/Area",
                    "Pronunciation"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    200,
                    50,
                    50,
                    100,
                    600
                },
                RequiredFields = new string[]
                {

                },
                Description = "Use this tok group for anything related to games.",
                Descriptions = new string[]
                {
                    "Any game that can be played on a cellular device",
                    "Any game that can be played on a PC ",
                    "Any game that can be played on a Gaming Console",
                    "Any game that can be played on a physical board",
                    "Any game that involves cards",
                    "Any other game that does not fit on the other game tok types"
                },
                Example = "",
                Examples = new string[]
                {
                    "Temple Run",
                    "League of Legends",
                    "Super Smash Bros.",
                    "Monopoly",
                    "Uno",
                    "Corn Hole"
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-game",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1548827727,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Qualities",
                PrimaryFieldName = "Name",
                SecondaryFieldName = "Quality",
                IsDetailBased = true,
                PrimaryCharLimit = 200,
                SecondaryCharLimit = 200,
                TokTypes = new string[]
                {
                    "Benefits of",
                    "Greatest and Least",
                    "Harms of",
                    "Pros ",
                    "Cons",
                    "Do's",
                    "Don'ts"
                },
                TokTypeIds = new string[]
                {
                    "toktype-qualities-benefitsof",
                    "toktype-qualities-greatestandleast",
                    "toktype-qualities-harmsof",
                    "toktype-qualities-pros",
                    "toktype-qualities-cons",
                    "toktype-qualities-dos",
                    "toktype-qualities-donts"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Sentence Example",
                    "Author/Attributed to",
                    "Time Period",
                    "Country/Region/Area",
                    "Pronunciation"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    200,
                    50,
                    50,
                    100,
                    600
                },
                RequiredFields = new string[]
                {

                },
                Description = "Use this for qualitative toks such as greatest, benefits, harms, etc.",
                Descriptions = new string[]
                {
                    "Information about how something can help you",
                    "Facts regarding the tallest,shortest, biggest, smallest, best, worst, etc.",
                    "Information about how something can be dangerous to you",
                    "The good parts about something",
                    "The bad parts about something",
                    "Good actions that can benefit you or others",
                    "Bad actions that can adversely affect you or others"
                },
                Example = "---",
                Examples = new string[]
                {
                    "Banana Peel Benefits: Helps get rid of warts, whitens teeth, eases poison ivy",
                    "Happiest Country in 2017:Norway",
                    "How alcohol harms: Weakens liver, Impairs thinking, causes cancer",
                    "Pros of living in a city: more jobs, more things to do, less driving",
                    "Cons of living in a city: density of people, not as peaceful, more pollution, more noise",
                    "Do drink 6-8 glasses of water every day.  It will keep you hydrated and: 1) help your physical performance 2) give you energy and help you mentally perform better 3) give you clearer skin 4) help relieve constipation 5) help prevent headaches",
                    "Don't rub your leg or butt against the car when you are loading gas.  The friction can cause a spark and cause an explosion. "
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-qualities",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1548827727,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Place",
                PrimaryFieldName = "Name",
                SecondaryFieldName = "Detail",
                IsDetailBased = true,
                PrimaryCharLimit = 200,
                SecondaryCharLimit = 200,
                TokTypes = new string[]
                {
                    "Country",
                    "City",
                    "Landmark",
                    "Tourist Spot",
                    "Historical",
                    "Region",
                    "Entertainment Venue",
                    "Restaurant",
                    "Bar",
                    "Coffee House",
                    "Other Place"
                },
                TokTypeIds = new string[]
                {
                    "toktype-place-country",
                    "toktype-place-city",
                    "toktype-place-landmark",
                    "toktype-place-touristspot",
                    "toktype-place-historical",
                    "toktype-place-region",
                    "toktype-place-entertainmentvenue",
                    "toktype-place-restaurant",
                    "toktype-place-bar",
                    "toktype-place-coffeehouse",
                    "toktype-place-otherplace"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Sentence Example",
                    "Author/Attributed to",
                    "Time Period",
                    "Country/Region/Area",
                    "Pronunciation"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    200,
                    50,
                    50,
                    100,
                    600
                },
                RequiredFields = new string[]
                {

                },
                Description = "Use this tok group for geographic locations, places to visit, etc.",
                Descriptions = new string[]
                {
                    "A nation with its own government and territory",
                    "A large town where people congregate to live",
                    "A famous place in a location",
                    "A location that tourists visit",
                    "A place with history",
                    "A area that has a common geography, or climate, or culture, or language, etc.",
                    "An location where entertainers perform",
                    "A business that serves food",
                    "A place where you can drink alcohol",
                    "An establishment that serves and sells coffee",
                    "Any other place"
                },
                Example = "",
                Examples = new string[]
                {
                    "China",
                    "New York City",
                    "Statue of Liberty-New York City",
                    "The Grand Canyon",
                    "Roman Colosseum",
                    "The Great Plains",
                    "Radio City Music Hall",
                    "Olive Garden",
                    "Harry's New York Bar",
                    "Starbucks Coffee",
                    "Juice Bar: Miss Lily's and Melvin's Juice Box"
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-place",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1548827727,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Person",
                PrimaryFieldName = "Name",
                SecondaryFieldName = "Detail",
                IsDetailBased = true,
                PrimaryCharLimit = 200,
                SecondaryCharLimit = 200,
                TokTypes = new string[]
                {
                    "Entertainer",
                    "Politician",
                    "Business",
                    "Athlete",
                    "Military",
                    "Religious",
                    "Writer",
                    "Scientist",
                    "Leader",
                    "Inventor",
                    "Fictional Character",
                    "Other Person"
                },
                TokTypeIds = new string[]
                {
                    "toktype-person-entertainer",
                    "toktype-person-politician",
                    "toktype-person-business",
                    "toktype-person-athlete",
                    "toktype-person-military",
                    "toktype-person-religious",
                    "toktype-person-writer",
                    "toktype-person-scientist",
                    "toktype-person-leader",
                    "toktype-person-inventor",
                    "toktype-person-fictionalcharacter",
                    "toktype-person-otherperson"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Sentence Example",
                    "Author/Attributed to",
                    "Time Period",
                    "Country/Region/Area",
                    "Pronunciation"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    200,
                    50,
                    50,
                    100,
                    600
                },
                RequiredFields = new string[]
                {

                },
                Description = "Use this tok group for entering toks about people",
                Descriptions = new string[]
                {
                    "A performer in the film, music, show industry,etc.",
                    "A political figure",
                    "An individual in any business industry",
                    "A person involved with athletics",
                    "Someone who served in the military",
                    "A religious figure",
                    "Someone who writes fiction or nonfiction",
                    "A person in the science field",
                    "Person who leads people",
                    "Someone who invented something",
                    "Character that does not exist in real life",
                    "Any other person"
                },
                Example = "",
                Examples = new string[]
                {
                    "Hugh Jackman",
                    "Hillary Clinton",
                    "Warren Buffet",
                    "Lebron James",
                    "James \"Mad Dog\" Mattis",
                    "Pope Francis",
                    "J.K. Rowling",
                    "Stephen Hawking",
                    "Steve Jobs-Apple",
                    "Thomas Edison",
                    "Superman",
                    "Criminal: Al Capone"
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-person",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1548827727,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Information",
                PrimaryFieldName = "Title",
                SecondaryFieldName = "Detail",
                IsDetailBased = true,
                PrimaryCharLimit = 200,
                SecondaryCharLimit = 200,
                TokTypes = new string[]
                {
                    "Online",
                    "Newsletter",
                    "Magazine",
                    "Newspaper",
                    "Book",
                    "Website",
                    "Subject",
                    "Topic",
                    "History",
                    "Other Information"
                },
                TokTypeIds = new string[]
                {
                    "toktype-information-online",
                    "toktype-information-newsletter",
                    "toktype-information-magazine",
                    "toktype-information-newspaper",
                    "toktype-information-book",
                    "toktype-information-website",
                    "toktype-information-subject",
                    "toktype-information-topic",
                    "toktype-information-history",
                    "toktype-information-otherinformation"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Country/Region/Area"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    100
                },
                RequiredFields = new string[]
                {

                },
                Description = "Use this for knowledge from information sources such as magazines, books, websites, etc.",
                Descriptions = new string[]
                {
                    "Information about an online source",
                    "Information about a newsletter",
                    "Information about a magazine",
                    "Information about a local newspaper or any major publication",
                    "Information about a book",
                    "Information about any website on the internet",
                    "Information about any subject",
                    "Information about any given topic",
                    "Information about a historical event",
                    "Information that does not fit under any Tok Types under the Tok Group \"Information\""
                },
                Example = "",
                Examples = new string[]
                {
                    "MSN: 1. Launched on August 29, 1995 2. Owned by Microsoft Corporation 3. Launched on the same date as Windows 95",
                    "The Daily Beast: 1. Launched October 6, 2008 2. Owned by IAC 3. Awarded the Digiday Publishing Award for Best Email Newsletter",
                    "Sports Illustrated: 1. Founded in 1954 2. Owned by Meredith Corporation. 3. Read by 23 million people each week.",
                    "The Washington Post: 1.Largest Newspaper published in Washington D.C. 2. Founded on December 6th, 1877 3. Eighth most circulated newspaper in the US",
                    "The Hobbit:1 Written by J.R.R. Tolkien 2. Also known as \"There and Back Again\" 3. Sixth best selling single-volume book of all time",
                    "Google: 1. Founded by Larry Page and Sergey Brin 2. Its headquarters are in Mountain View, CA 3. It is the worlds most popular search engine.",
                    "Math: 1.  It is the study of quantity, structure, space, and change 2. Involves problem solving, numbers, etc. 3. Calculators are used to solve int math problems.",
                    "Ebola Virus: 1. Causes severe bleeding, organ failure, and can lead to death. 2. Humans may spread the virus to other humans through contact with bodily fluids such as blood.  3. Symptoms include fever, headache, muscle pain, and chills.",
                    "World War II: 1. Started in 1939 2. Ended in 1945 3. Won by the Allied Forces",
                    "Acne Medicine Ingredients: 1. Benzoyl Peroxide 2. Salicylic acid 3. Alpha Hydroxy acids 4. Sulfur"
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-information",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1548827727,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Image",
                PrimaryFieldName = "Name",
                SecondaryFieldName = "Detail",
                IsDetailBased = true,
                PrimaryCharLimit = 200,
                SecondaryCharLimit = 200,
                TokTypes = new string[]
                {
                    "Sign",
                    "Flag",
                    "Logo",
                    "Symbol",
                    "Photo",
                    "Drawing",
                    "Painting",
                    "Graphic Art",
                    "Map",
                    "Meme",
                    "Other Image"
                },
                TokTypeIds = new string[]
                {
                    "toktype-image-sign",
                    "toktype-image-flag",
                    "toktype-image-logo",
                    "toktype-image-symbol",
                    "toktype-image-photo",
                    "toktype-image-drawing",
                    "toktype-image-painting",
                    "toktype-image-graphicart",
                    "toktype-image-map",
                    "toktype-image-meme",
                    "toktype-image-otherimage"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Author/Attributed to",
                    "Time Period",
                    "Country/Region/Area"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    50,
                    50,
                    100
                },
                RequiredFields = new string[]
                {

                },
                Description = "A picture,drawing, or other graphic.",
                Descriptions = new string[]
                {
                    "Image of a street sign, billboard, etc.",
                    "Image of a flag from any country or organization",
                    "Image of any logo from any company",
                    "Image of any symbol",
                    "Image of any photo captured by a camera",
                    "Image of any drawing",
                    "Image of a famous painting",
                    "Any computer designed graphic",
                    "Image of a map of any country, city, place, etc.",
                    "Image of a notable meme",
                    "Does not fit under another image tok type"
                },
                Example = "",
                Examples = new string[]
                {
                    "Image of a stop sign",
                    "USA Flag",
                    "Ferarri Logo",
                    "Peace symbol",
                    "Photo of Mount Rushmore",
                    "Image of a sketch",
                    "Mona Lisa",
                    "Clip Art",
                    "Map of China ",
                    "Bad Luck Brian Image",
                    "tapestry"
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-image",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1548827727,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Humor",
                PrimaryFieldName = "First Line",
                SecondaryFieldName = "Punchline",
                IsDetailBased = false,
                PrimaryCharLimit = 200,
                SecondaryCharLimit = 200,
                TokTypes = new string[]
                {
                    "Joke",
                    "Pickup Line"
                },
                TokTypeIds = new string[]
                {
                    "toktype-humor-joke",
                    "toktype-humor-pickupline"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Time Period",
                    "Country/Region/Area"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    50,
                    100
                },
                RequiredFields = new string[]
                {

                },
                Description = "Use this group for jokes or pick up lines",
                Descriptions = new string[]
                {
                    "Humorous questions and answers",
                    "Flirtatious saying or question to spark interest"
                },
                Example = "---",
                Examples = new string[]
                {
                    "Q: Where do boats go when they are sick? A: The Dock",
                    "Hey, I lost my phone number; can I have yours instead?"
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-humor",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1548827727,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Abbreviation",
                PrimaryFieldName = "Abbreviation",
                SecondaryFieldName = "Meaning",
                IsDetailBased = false,
                PrimaryCharLimit = 25,
                SecondaryCharLimit = 200,
                TokTypes = new string[]
                {
                    "Acronym",
                    "Initialism",
                    "One Word Abbreviation",
                    "Other Abbreviation"
                },
                TokTypeIds = new string[]
                {
                    "toktype-abbreviation-acronym",
                    "toktype-abbreviation-initialism",
                    "toktype-abbreviation-onewordabbreviation",
                    "toktype-abbreviation-otherabbreviation"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Sentence Example",
                    "Author/Attributed to",
                    "Time Period",
                    "Country/Region/Area",
                    "Pronunciation"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    200,
                    50,
                    50,
                    100,
                    600
                },
                RequiredFields = new string[]
                {
                    "Country",
                    "State/Region/City"
                },
                Description = "Any shortened form of a word or words",
                Descriptions = new string[]
                {
                    "Abbreviation that is pronounced as a word",
                    "Abbreviation consisting of initial letters pronounced separately",
                    "A shortened form of a word",
                    "Other Abbreviation that doesn't beint in one of the types"
                },
                Example = "NATO, USA, CA, Ms.",
                Examples = new string[]
                {
                    "NATO - North Atlantic Treaty Organization, SCUBA - Self-contained Underwater Breathing Apparatus",
                    "USA - United States of America, BMW - Bayerische Motoren Werke",
                    "abbreviation - abbr., abbrv., or abbrev 2) CA -  California 3) MM - million",
                    "(no example)"
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-abbreviation",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1548827727,

            },
            new TokTypeList()
            {
                Label = "toktypelist",
                TokGroup = "Entertainment",
                PrimaryFieldName = "Name",
                SecondaryFieldName = "Detail",
                IsDetailBased = true,
                PrimaryCharLimit = 200,
                SecondaryCharLimit = 200,
                TokTypes = new string[]
                {
                    "Exhibit/Museum",
                    "Fair/Festival",
                    "Performance",
                    "TV",
                    "Radio",
                    "Music",
                    "Movie",
                    "Musical",
                    "Play",
                    "Opera",
                    "Show",
                    "Theme Park",
                    "Animation",
                    "Orchestra",
                    "Dance",
                    "Song",
                    "Event",
                    "Other Entertainment",
                    "Magazine"
                },
                TokTypeIds = new string[]
                {
                    "toktype-entertainment-exhibitmuseum",
                    "toktype-entertainment-fairfestival",
                    "toktype-entertainment-performance",
                    "toktype-entertainment-tv",
                    "toktype-entertainment-radio",
                    "toktype-entertainment-music",
                    "toktype-entertainment-movie",
                    "toktype-entertainment-musical",
                    "toktype-entertainment-play",
                    "toktype-entertainment-opera",
                    "toktype-entertainment-show",
                    "toktype-entertainment-themepark",
                    "toktype-entertainment-animation",
                    "toktype-entertainment-orchestra",
                    "toktype-entertainment-dance",
                    "toktype-entertainment-song",
                    "toktype-entertainment-event",
                    "toktype-entertainment-otherentertainment",
                    "toktype-entertainment-magazine"
                },
                OptionalFields = new string[]
                {
                    "Age Group",
                    "Source",
                    "Sentence Example",
                    "Author/Attributed to",
                    "Time Period",
                    "Country/Region/Area",
                    "Pronunciation"
                },
                OptionalLimits = new long[]
                {
                    50,
                    300,
                    200,
                    50,
                    50,
                    100,
                    600
                },
                RequiredFields = new string[]
                {

                },
                Description = "This tok group has entertainment related toks such as events, shows, music, etc.",
                Descriptions = new string[]
                {
                    "Anything about an exhibit or museum",
                    "Anything about a fair or festival",
                    "Anything about a performance",
                    "Anything about a television show",
                    "Anything about radio entertainment",
                    "Anything related to music",
                    "Anything about a movie",
                    "Anything about a musical",
                    "Anything about a play",
                    "Anything about an opera",
                    "Anything about a live show",
                    "Anything about a theme park",
                    "Anything related to an animated movie or TV show",
                    "Anything about orchestra",
                    "Anything about a dance",
                    "Anything about a song",
                    "Anything about an event",
                    "Any other form of entertainment",
                    "Anything about a magazine"
                },
                Example = "",
                Examples = new string[]
                {
                    "Smithsonian Museum",
                    "Oktoberfest",
                    "Beyonce Super Bowl XLVII performance",
                    "American Idol",
                    "The Rush Limbaugh Show",
                    "Rap Music",
                    "Avatar",
                    "Les Miserables",
                    "Macbeth",
                    "The Magic Flute",
                    "Cirque du Soleil",
                    "Disneyland",
                    "The Incredibles",
                    "San Francisco Symphony",
                    "Ballet",
                    "Despacito by Luis Fonsi",
                    "4th of July fireworks",
                    "Karaoke Nightclub",
                    "Time Magazine"
                },
                HasAnswerField = false,
                DetailsMin = 3,
                DetailsMax = 5,
                DetailsDefault = 5,
                Id = "toktypelist-entertainment",
                PartitionKey = "toktypelist",
                SoftMarker = null,
                _Timestamp = 1548827676,

            },
        };
    }
}
