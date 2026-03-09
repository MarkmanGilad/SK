# Copilot Instructions

## Project Guidelines
- User is using OpenAI SDK and Gemini SDK directly, not Semantic Kernel, for their SQL tool integration.
- User prefers simpler code changes and wants to avoid overly complex helper methods when a simpler built-in API option exists. 
- User prefers simpler code changes and wants helper/extraction methods removed when a simpler direct approach is possible.
- User prefers resolving project-relative folders directly from `AppContext.BaseDirectory` with `..\\..\\..` instead of using a `GetProjectDirectory` helper when working in this project.
- User explicitly prefers changing the saved images collection to a public list instead of a private list plus IReadOnlyList wrapper in this file.
- User wants to remove the `LastSavedImages` state and return saved images directly from the `Call` instead.

## Image Workflow Guidelines
- User wants the Lesson_9_images_Voice_2 image workflow simplified: OpenAI_Tools should revert to a generic wrapper that only saves images if produced.
- GPT_Create_Image and GPT_Edit_Image should be completely separate.
- GPT_Edit_Image should use a hard-coded image path instead of chaining from GPT_Create_Image.