describe("Playlist Tests", () => {
  beforeEach(() => {
    cy.visit("http://localhost:5173/main");
  });

  it("Displays playlist elements correctly", () => {
    cy.url().should("include", "localhost:5173");
    cy.get("[data-testid^='playlist-list__card-']").first().click();

    cy.getId("playlist-display-title").should("be.visible");
    cy.getId("playlist-display-image").should("be.visible");
    cy.getId("playlist-display-songs-container").should("be.visible");
  });

  it("Allows adding and deleting a song in a playlist", () => {
    // Select playlist deterministically
    cy.get("select").select("1");

    // Add a song
    cy.getId("song-search-input").type("pupa");
    cy.getId("song-search-button").click();

    // Click the 'Add' button for the first search result
    cy.getId("song-add-btn").first().click();

    cy.on("window:alert", (text) => {
      expect(text).to.eq("Song added successfully!");
    });

    // Verify it appears in the playlist
    cy.reload();
    cy.get("[data-testid^='playlist-list__card-']").first().click();
    cy.getId("playlist-display-songs-container").should("contain", "pupa");

    // Delete the song
    cy.getId("trash").click();
  });

  it("Prevents adding song when no playlist is selected", () => {
    cy.getId("song-search-input").type("pupa");
    cy.getId("song-search-button").click();
    cy.getId("song-add-btn").first().click();

    cy.on("window:alert", (text) => {
      expect(text).to.eq("Please select a playlist first!");
    });
  });

  it("Allows creating a new playlist", () => {
    cy.getId("new-playlist-btn").click();
    cy.get('[placeholder="Playlist name"]').type("test playlist");
    cy.get('[placeholder="Description"]').type("temporary test playlist");
    cy.get("button").contains("Create").click();

    // Verify playlist exists
    cy.get(".playlist-card").contains("test playlist").should("be.visible");
  });
});
