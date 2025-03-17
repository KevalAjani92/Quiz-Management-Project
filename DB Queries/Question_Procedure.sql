USE CSE_23010101006;


-- Insert into MST_Question
CREATE PROCEDURE [dbo].[PR_Question_Insert]
    @QuestionText		NVARCHAR(MAX),
    @QuestionLevelID	INT,
    @OptionA			NVARCHAR(100),
    @OptionB			NVARCHAR(100),
    @OptionC			NVARCHAR(100) = NULL,
    @OptionD			NVARCHAR(100) = NULL,
    @CorrectOption		NVARCHAR(100),
    @QuestionMarks		INT,
    @IsActive			BIT = 1,
    @UserID				INT
AS
INSERT INTO [dbo].[MST_Question] 
(
	[QuestionText],
	[QuestionLevelID],
	[OptionA],
	[OptionB],
	[OptionC],
	[OptionD],
	[CorrectOption],
	[QuestionMarks],
	[IsActive],
	[UserID]
)
VALUES
(
	@QuestionText,
	@QuestionLevelID,
	@OptionA,
	@OptionB,
	@OptionC,
	@OptionD,
	@CorrectOption,
	@QuestionMarks,
	@IsActive,
	@UserID
);


-- Update MST_Question
CREATE or Alter PROCEDURE [dbo].[PR_Question_UpdateByPK]
    @QuestionID			INT,
    @QuestionText		NVARCHAR(MAX),
    @QuestionLevelID	INT,
    @OptionA			NVARCHAR(100),
    @OptionB			NVARCHAR(100),
    @OptionC			NVARCHAR(100) = NULL,
    @OptionD			NVARCHAR(100) = NULL,
    @CorrectOption		NVARCHAR(100),
    @QuestionMarks		INT,
    @IsActive			BIT,
    @UserID				INT
AS
UPDATE [dbo].[MST_Question]
	SET [QuestionText] = @QuestionText,
		[QuestionLevelID] = @QuestionLevelID,
		[OptionA] = @OptionA,
		[OptionB] = @OptionB,
		[OptionC] = @OptionC,
		[OptionD] = @OptionD,
		[CorrectOption] = @CorrectOption,
		[QuestionMarks] = @QuestionMarks,
		[IsActive] = @IsActive,
		[UserID] = @UserID,
		[Modified] = GETDATE()
WHERE [dbo].[MST_Question].[QuestionID] = @QuestionID;


-- Delete from MST_Question
CREATE PROCEDURE [dbo].[PR_Question_DeleteByPK]
    @QuestionID INT
AS
DELETE 
FROM [dbo].[MST_Question]
WHERE [dbo].[MST_Question].[QuestionID] = @QuestionID;


-- Select All from MST_Question
CREATE or Alter PROCEDURE [dbo].[PR_Question_SelectAll]
    @QuestionLevel VARCHAR(100) = NULL,
    @QuestionText  VARCHAR(MAX) = NULL,
    @MinMarks      INT          = NULL,
    @MaxMarks      INT          = NULL
AS
    SELECT [dbo].[MST_Question].[QuestionID],
           [dbo].[MST_Question].[QuestionText],
           [dbo].[MST_Question].[QuestionLevelID],
		   [dbo].[MST_QuestionLevel].[QuestionLevel],
           [dbo].[MST_Question].[OptionA],
           [dbo].[MST_Question].[OptionB],
           [dbo].[MST_Question].[OptionC],
           [dbo].[MST_Question].[OptionD],
           [dbo].[MST_Question].[CorrectOption],
           [dbo].[MST_Question].[QuestionMarks],
           [dbo].[MST_Question].[IsActive],
           [dbo].[MST_Question].[UserID],
		   [dbo].[MST_User].[UserName],
           Format([dbo].[MST_Question].[Created],'dd-MM-yyyy') as Created,
           [dbo].[MST_Question].[Modified]
    FROM [dbo].[MST_Question]

	INNER JOIN [dbo].[MST_User]
	ON [dbo].[MST_User].[UserID] = [dbo].[MST_Question].[UserID]

	INNER JOIN [dbo].[MST_QuestionLevel]
	ON [dbo].[MST_QuestionLevel].[QuestionLevelID] = [dbo].[MST_Question].[QuestionLevelID]

    WHERE
        (@QuestionLevel Is NULL OR [dbo].[MST_QuestionLevel].[QuestionLevel] Like '%'+@QuestionLevel+'%')
        AND
        (@QuestionText Is NULL OR [dbo].[MST_Question].[QuestionText] Like '%'+@QuestionText+'%')
        AND
        (@MinMarks Is NULL OR [dbo].[MST_Question].[QuestionMarks] >= @MinMarks)
        AND
        (@MaxMarks IS Null OR [dbo].[MST_Question].[QuestionMarks] <= @MaxMarks)

	ORDER BY [dbo].[MST_Question].[QuestionID],
			 [dbo].[MST_QuestionLevel].[QuestionLevel],
			 [dbo].[MST_User].[UserName]

EXEC PR_Question_SelectAll

-- Select by Primary Key from MST_Question
CREATE or ALTER PROCEDURE [dbo].[PR_Question_SelectByPK]
    @QuestionID INT
AS
    SELECT [dbo].[MST_Question].[QuestionID],
           [dbo].[MST_Question].[QuestionText],
           [dbo].[MST_Question].[QuestionLevelID],
		   [dbo].[MST_QuestionLevel].[QuestionLevel],
           [dbo].[MST_Question].[OptionA],
           [dbo].[MST_Question].[OptionB],
           [dbo].[MST_Question].[OptionC],
           [dbo].[MST_Question].[OptionD],
           [dbo].[MST_Question].[CorrectOption],
           [dbo].[MST_Question].[QuestionMarks],
           [dbo].[MST_Question].[IsActive],
           [dbo].[MST_Question].[UserID],
           [dbo].[MST_Question].[Created],
           [dbo].[MST_Question].[Modified]
    FROM [dbo].[MST_Question]

	Inner Join [dbo].[MST_QuestionLevel]
	On [dbo].[MST_QuestionLevel].[QuestionLevelID] = [dbo].[MST_Question].[QuestionLevelID]

    WHERE [dbo].[MST_Question].[QuestionID] = @QuestionID;

---QUESTION DROPDOW
CREATE PROCEDURE [dbo].[PR_Question_Dropdown]
AS
	SELECT
		 [dbo].[MST_Question].[QuestionID],
		 [dbo].[MST_Question].[QuestionText]
	FROM [dbo].[MST_Question]
