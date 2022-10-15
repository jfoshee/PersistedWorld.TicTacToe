/** Take Turn */
function mutate(context, squareNumber) {
  let entity = context.entity;
  // The player taking the turn is available on the context as the current user ID
  let playerId = context.userId;
  let state = entity.customStatePublic[context.authorId];
  if (state.type !== 'TicTacToeBoard')
    throw Error('TakeTurn should only be invoked on a TicTacToeBoard');
  if (state.isComplete)
    throw Error('The game is complete');
  if (state.turn !== playerId)
    throw Error('Not your turn');
  let shape = '?';
  if (state.players[0] === playerId) {
    shape = 'x';
    state.turn = state.players[1];
  }
  else if (state.players[1] === playerId) {
    shape = 'o';
    state.turn = state.players[0];
  }
  else
    throw Error('Not a player');
  state.message = `It is ${state.turn}'s turn.`
  if (state.board[squareNumber] !== ' ')
    throw Error('The square is already taken');
  // HACK: Must re-create the board array in order to change an element!
  let board = Array.from(state.board);
  board[squareNumber] = shape;
  state.board = board;

  let winningRows = [
    // Horizontal
    [0, 1, 2],
    [3, 4, 5],
    [6, 7, 8],
    // Vertical
    [0, 3, 6],
    [1, 4, 7],
    [2, 5, 8],
    // Diagonal
    [0, 4, 8],
    [2, 4, 6]
  ];
  for (const winningRow of winningRows) {
    let a = winningRow[0];
    let b = winningRow[1];
    let c = winningRow[2];
    if (board[a] === board[b] && board[b] === board[c] && board[a] !== ' ') {
      // we have a winner
      let winningShape = board[a];
      if (winningShape === 'x')
        state.winner = state.players[0];
      else
        state.winner = state.players[1];
      state.isComplete = true;
      state.message = `${state.winner} Wins!`;
      break;
    }
  }
  // If no winner, check if the board is full
  if (!state.winner && board.every(square => square !== ' ')) {
    state.isComplete = true;
    state.winner = null;
    state.message = `Tie Game!`;
  }
  // Update stats on User (Player1) entity
  if (state.winner == context.userId) {
    let userState = context.user.customStatePublic[context.authorId];
    userState.wins = +userState.wins || 0;
    userState.wins++;
  }

  log(state.message);
  // TODO: Update stats on Creator entity
  // TODO: Use DisplayNames for players in messages
}
