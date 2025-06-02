using ToSic.Eav.Apps.Internal.Api01;
using ToSic.Lib.Logging;

namespace ToSic.Eav.Apps.Tests.Api.Api01;


// ReSharper disable once InconsistentNaming
public class SimpleDataControllerTests_IsDraft
{
    private static (bool ShouldPublish, bool ShouldBranchDrafts) TestGetPublishSpecs(object publishedState, bool? existingIsPublished, bool writePublishAllowed)
    {
        var r = SimpleDataEditService.GetPublishSpecs(publishedState, existingIsPublished, writePublishAllowed,
            new Log("test"));
        return (r.ShouldPublish, r.ShouldBranchDrafts);
    }

    /// <summary>
    /// Scenarios when creating new.
    /// 1.	No published state – result should be default,
    /// so depending on user permissions it's published or draft.
    /// This should be the same as previous implementation. 
    /// </summary>
    /// <param name="publishedState"></param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("null")]
    [InlineData("NULL")]
    [InlineData("NUll")]
    public void New_NoPublishedState(object publishedState)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState, 
            existingIsPublished: null,
            writePublishAllowed: true);

        True(published); // true is default
        False(branch); // false because is it new one, so no branch
    }

    /// <summary>
    /// Scenarios when creating new.
    /// 1.	No published state – result should be default,
    /// so depending on user permissions it's published or draft.
    /// This should be the same as previous implementation. 
    /// </summary>
    /// <param name="publishedState"></param>
    [Theory]
    [InlineData(null)]
    public void New_NoPublishedState_WritePublishNotAllowed(object publishedState)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: null,
            writePublishAllowed: false);

        False(published); // false because publish is not allowed
        False(branch); // false because is it new one, so no branch
    }

    /// <summary>
    /// Scenarios when creating new.
    /// 2.	"true", 1 etc. – should be published, assuming user permissions allow this.
    /// Basically it should not set the published for saving at all, as the default is true,
    /// and permissions may not work if you set it.
    /// </summary>
    /// <param name="publishedState"></param>
    [Theory]
    [InlineData(true)]
    [InlineData(1)]
    [InlineData("true")]
    [InlineData("TRUE")]
    [InlineData("TRue")]
    public void New_True(object publishedState)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: null,
            writePublishAllowed: true);

        True(published); // true (and publish is allowed)
        False(branch); // false because is it new one, so no branch
    }

    /// <summary>
    /// Scenarios when creating new.
    /// 2.	"true", 1 etc. – should be published, assuming user permissions allow this.
    /// Basically it should not set the published for saving at all, as the default is true,
    /// and permissions may not work if you set it.
    /// </summary>
    /// <param name="publishedState"></param>
    [Theory]
    [InlineData(true)]
    public void New_True_WritePublishNotAllowed(object publishedState)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: null,
            writePublishAllowed: false);

        False(published); // false because publish is not allowed
        False(branch); // false because is it new one, so no branch
    }

    /// <summary>
    /// Scenarios when creating new.
    /// 3.	False, 0, etc. – should not be published
    /// </summary>
    /// <param name="publishedState"></param>
    [Theory]
    [InlineData(false)]
    [InlineData(0)]
    [InlineData("false")]
    [InlineData("FALSE")]
    [InlineData("FAlse")]
    public void New_False(object publishedState)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: null,
            writePublishAllowed: true);

        False(published); // false and publish is allowed
        False(branch); // false because is it new one, so no branch
    }

    /// <summary>
    /// Scenarios when creating new.
    /// 3.	False, 0, etc. – should not be published
    /// </summary>
    /// <param name="publishedState"></param>
    [Theory]
    [InlineData(false)]
    public void New_False_WritePublishNotAllowed(object publishedState)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: null,
            writePublishAllowed: false);

        False(published); // false because publish is not allowed
        False(branch); // false because is it new one, so no branch
    }

    /// <summary>
    /// Existing data which is published
    /// 1.	No published state – unchanged, shouldn't set at all, because if user permisisons restrict to draft-only,
    /// that should happen automatically. Result
    /// a.	If user is allowed to publish, it's published
    /// </summary>
    /// <param name="publishedState"></param>
    /// <param name="existingIsPublished"></param>
    /// <param name="writePublishAllowed"></param>
    [Theory]
    [InlineData(null, true, true)]
    public void ExistingPublished_NoPublishedState(object publishedState, bool? existingIsPublished, bool writePublishAllowed)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: existingIsPublished,
            writePublishAllowed: writePublishAllowed);

        True(published); // true because existing is published and save publish is allowed
        False(branch); // false because is it allowed to save publish
    }

    /// <summary>
    /// Existing data which is published
    /// 1.	No published state – unchanged, shouldn't set at all, because if user permisisons restrict to draft-only,
    /// that should happen automatically. Result
    /// b.	If user is draft-only, it's should draft/fork
    /// </summary>
    /// <param name="publishedState"></param>
    /// <param name="existingIsPublished"></param>
    /// <param name="writePublishAllowed"></param>
    [Theory]
    [InlineData(null, true, false)]
    public void ExistingPublished_NoPublishedState_WritePublishNotAllowed(object publishedState, bool? existingIsPublished, bool writePublishAllowed)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: existingIsPublished,
            writePublishAllowed: writePublishAllowed);

        False(published); // false because save publish is not allowed
        True(branch); // true because save publish is not allowed
    }

    /// <summary>
    /// Existing data which is published
    /// 2.	True, 1, etc. – unchanged, shouldn't set at all, again because of user permissions. Result
    /// a.	If user is allowed to publish, it's published
    /// </summary>
    /// <param name="publishedState"></param>
    /// <param name="existingIsPublished"></param>
    /// <param name="writePublishAllowed"></param>
    [Theory]
    [InlineData(true, true, true)]
    public void ExistingPublished_True(object publishedState, bool? existingIsPublished, bool writePublishAllowed)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: existingIsPublished,
            writePublishAllowed: writePublishAllowed);

        True(published); // true (and save publish is allowed)
        False(branch); // false because save publish is allowed
    }

    /// <summary>
    /// Existing data which is published
    /// 2.	True, 1, etc. – unchanged, shouldn't set at all, again because of user permissions. Result
    /// b.	If user is draft-only, it's should draft/fork
    /// </summary>
    /// <param name="publishedState"></param>
    /// <param name="existingIsPublished"></param>
    /// <param name="writePublishAllowed"></param>
    [Theory]
    [InlineData(true, true, false)]
    public void ExistingPublished_True_WritePublishNotAllowed(object publishedState, bool? existingIsPublished, bool writePublishAllowed)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: existingIsPublished,
            writePublishAllowed: writePublishAllowed);

        False(published); // false because save publish is not allowed
        True(branch); // true because save publish is not allowed
    }

    /// <summary>
    /// Existing data which is published
    /// 3.	False: depending on user permissions. Result
    /// b.	If user is allowed to change published (full write permissions), then this should result in no publish, no branch
    /// </summary>
    /// <param name="publishedState"></param>
    /// <param name="existingIsPublished"></param>
    /// <param name="writePublishAllowed"></param>
    [Theory]
    [InlineData(false, true, true)]
    public void ExistingPublished_False(object publishedState, bool? existingIsPublished, bool writePublishAllowed)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: existingIsPublished,
            writePublishAllowed: writePublishAllowed);

        False(published); // false (also save publish is allowed)
        False(branch); // false because save publish is allowed
    }

    /// <summary>
    /// Existing data which is published
    /// 3.	False: depending on user permissions. Result
    /// a.	If user can only create draft, then it should branch, as the user is not allowed to affect published
    /// </summary>
    /// <param name="publishedState"></param>
    /// <param name="existingIsPublished"></param>
    /// <param name="writePublishAllowed"></param>
    [Theory]
    [InlineData(false, true, false)]
    public void ExistingPublished_False_WritePublishNotAllowed(object publishedState, bool? existingIsPublished, bool writePublishAllowed)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: existingIsPublished,
            writePublishAllowed: writePublishAllowed);

        False(published); // false because save publish is not allowed
        True(branch); // true because save publish is not allowed
    }

    /// <summary>
    /// Existing data which is published
    /// 4.	"draft" – result always draft and no published
    /// </summary>
    /// <param name="publishedState"></param>
    /// <param name="existingIsPublished"></param>
    /// <param name="writePublishAllowed"></param>
    [Theory]
    [InlineData("draft", true, true)]
    public void ExistingPublished_Draft(object publishedState, bool? existingIsPublished, bool writePublishAllowed)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: existingIsPublished,
            writePublishAllowed: writePublishAllowed);

        False(published); // false (even that publish is allowed)
        True(branch); // true because result always draft
    }

    /// <summary>
    /// Existing data which is published
    /// 4.	"draft" – result always draft and no published
    /// </summary>
    /// <param name="publishedState"></param>
    /// <param name="existingIsPublished"></param>
    /// <param name="writePublishAllowed"></param>
    [Theory]
    [InlineData("draft", true, false)]
    public void ExistingPublish_Draft_WritePublishNotAllowed(object publishedState, bool? existingIsPublished, bool writePublishAllowed)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: existingIsPublished,
            writePublishAllowed: writePublishAllowed);

        False(published); // false because save publish is not allowed
        True(branch); // true because save publish is not allowed
    }

    /// <summary>
    /// Existing data which is draft ONLY
    /// 1.	No state: unchanged, should remain draft
    /// </summary>
    /// <param name="publishedState"></param>
    /// <param name="existingIsPublished"></param>
    /// <param name="writePublishAllowed"></param>
    [Theory]
    [InlineData(null, false, true)]
    [InlineData(null, false, false)]
    public void ExistingDraft_NoPublishedState(object publishedState, bool? existingIsPublished, bool writePublishAllowed)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: existingIsPublished,
            writePublishAllowed: writePublishAllowed);

        False(published); // false, because it is draft
        False(branch); // false (it is already draft)
    }

    /// <summary>
    /// Existing data which is draft ONLY
    /// 2.	True, 1, etc. – should save changes and publish
    /// b.	If user is allowed to save published, then publish should happen
    /// </summary>
    /// <param name="publishedState"></param>
    /// <param name="existingIsPublished"></param>
    /// <param name="writePublishAllowed"></param>
    [Theory]
    [InlineData(true, false, true)]
    public void ExistingDraft_True(object publishedState, bool? existingIsPublished, bool writePublishAllowed)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: existingIsPublished,
            writePublishAllowed: writePublishAllowed);

        True(published); // true because it is allowed to save publish
        False(branch); // false because it is allowed to save publish
    }

    /// <summary>
    /// Existing data which is draft ONLY
    /// 2.	True, 1, etc. – should save changes and publish
    /// a.	If user is only allowed to create drafts, then publishing should not happen, draft only
    /// </summary>
    /// <param name="publishedState"></param>
    /// <param name="existingIsPublished"></param>
    /// <param name="writePublishAllowed"></param>
    [Theory]
    [InlineData(true, false, false)]
    public void ExistingDraft_True_WritePublishNotAllowed(object publishedState, bool? existingIsPublished, bool writePublishAllowed)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: existingIsPublished,
            writePublishAllowed: writePublishAllowed);

        False(published); // false because it is not allowed to save publish
        False(branch); // false because it already a draft
    }

    /// <summary>
    /// Existing data which is draft ONLY
    /// 3.	False, 0, etc. – Result always draft and no published
    ///  </summary>
    /// <param name="publishedState"></param>
    /// <param name="existingIsPublished"></param>
    /// <param name="writePublishAllowed"></param>
    [Theory]
    [InlineData(false, false, true)]
    [InlineData(false, false, false)]
    public void ExistingDraft_False(object publishedState, bool? existingIsPublished, bool writePublishAllowed)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: existingIsPublished,
            writePublishAllowed: writePublishAllowed);

        False(published); // false because no publish
        False(branch); // false because it is already draft
    }

    /// <summary>
    /// Existing data which is draft ONLY
    /// 4.	"draft" – result always draft and no published
    /// </summary>
    /// <param name="publishedState"></param>
    /// <param name="existingIsPublished"></param>
    /// <param name="writePublishAllowed"></param>
    [Theory]
    [InlineData("DRAFT", false, true)]
    [InlineData("DRaft", false, false)]
    public void ExistingDraft_Draft(object publishedState, bool? existingIsPublished, bool writePublishAllowed)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: existingIsPublished,
            writePublishAllowed: writePublishAllowed);

        False(published); // false because no publish
        False(branch); // false because it is already draft
    }

    /// <summary>
    /// Existing Data which is draft and published
    /// 1.	No state: unchanged, the updated data should only be in the draft
    /// </summary>
    /// <param name="publishedState"></param>
    /// <param name="existingIsPublished"></param>
    /// <param name="writePublishAllowed"></param>
    [Theory]
    [InlineData(null, false, true)]
    [InlineData(null, false, false)]
    public void ExistingDraftAndPublish_NoPublishedState(object publishedState, bool? existingIsPublished, bool writePublishAllowed)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: existingIsPublished,
            writePublishAllowed: writePublishAllowed);

        // the updated data should only be in the draft
        False(published); // false, because it is draft
        False(branch); // false (it is already draft)
    }

    /// <summary>
    /// Existing Data which is draft and published
    /// 2.	True: Result 
    /// a.	If user may write published, then all is now published
    /// </summary>
    /// <param name="publishedState"></param>
    /// <param name="existingIsPublished"></param>
    /// <param name="writePublishAllowed"></param>
    [Theory]
    [InlineData(true, false, true)]
    public void ExistingDraftAndPublish_True(object publishedState, bool? existingIsPublished, bool writePublishAllowed)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: existingIsPublished,
            writePublishAllowed: writePublishAllowed);

        // draft become published
        True(published); // true because it is allowed to save publish
        False(branch); // false because it is allowed to save publish
    }

    /// <summary>
    /// Existing Data which is draft and published
    /// 2.	True: Result 
    /// b.	If user may not do that, it's ignored, draft is updated only
    /// </summary>
    /// <param name="publishedState"></param>
    /// <param name="existingIsPublished"></param>
    /// <param name="writePublishAllowed"></param>
    [Theory]
    [InlineData(true, false, false)]
    public void ExistingDraftAndPublish_True_WritePublishNotAllowed(object publishedState, bool? existingIsPublished, bool writePublishAllowed)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: existingIsPublished,
            writePublishAllowed: writePublishAllowed);
            
        // draft is updated only
        False(published); // false because it is not allowed to save publish
        False(branch); // false because it is already a draft
    }

    /// <summary>
    /// Existing Data which is draft and published
    /// 3.	False: Result based on permissions
    /// a.	User may change published: all is now unpublished
    /// b.	User may not change published: ignore; update the draft only
    ///  </summary>
    /// <param name="publishedState"></param>
    /// <param name="existingIsPublished"></param>
    /// <param name="writePublishAllowed"></param>
    [Theory]
    [InlineData(false, false, true)]
    [InlineData(false, false, false)]
    public void ExistingDraftAndPublish_False(object publishedState, bool? existingIsPublished, bool writePublishAllowed)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: existingIsPublished,
            writePublishAllowed: writePublishAllowed);
            
        // publish is unpublished (or draft update only)
        False(published); // false because no publish
        False(branch); // false because it is already draft
    }

    /// <summary>
    /// Existing Data which is draft and published
    /// 4.	"draft" Result always draft and published, only draft was updated
    /// </summary>
    /// <param name="publishedState"></param>
    /// <param name="existingIsPublished"></param>
    /// <param name="writePublishAllowed"></param>
    [Theory]
    [InlineData("DRAFT", false, true)]
    [InlineData("DRaft", false, false)]
    public void ExistingDraftAndPublish_Draft(object publishedState, bool? existingIsPublished, bool writePublishAllowed)
    {
        var (published, branch) = TestGetPublishSpecs(
            publishedState: publishedState,
            existingIsPublished: existingIsPublished,
            writePublishAllowed: writePublishAllowed);

        // only draft was updated
        False(published); // false because no publish
        False(branch); // false because it is already draft
    }
}