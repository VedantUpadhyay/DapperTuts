let bookName, authorName, isbn;

var updatingBookId;

var globalBookId = 0;
let isUpdating = false;

let booksList = [];
let currentUpdatingRow;

const INSERT = 'INSERT';
const UPDATE = 'UPDATE';
const DELETE = 'DELETE';

/**
 * Single Operation defined as : -
 * JS Object - 
 * INSERTION
 * {
 *    operation: const,
 *    obj: {
 *      id: 1,
 *      bookName: 'abc',
 *      authorName: 'xyz',
 *      isbn: '1212'
 *    }
 * }
 * UPDATE
 * {
 *      operation: const,
 *      obj: {
 *          ...
 *      }
 *  }
 * DELETE 
 * {
 *      operation: const,
 *      obj : {
 *          id: bookId
 *      }
 * }
 * */

let operationsQ = [];

/**
 * Book example : - 
 * {
 *      id : {
 *            bookName: 'abc',
 *            authorName: 'xyz',
 *            isbn: '1212'
 *      }
 * }
 * */

let booksArr = {};

async function setGlobalValues(flag) {
    bookName = $("#bookName").val();
    authorName = $("#authorName").val();
    isbn = $("#isbn").val();

    if (flag) {
        updatingBookId = Number($("#bookId").val());
    }
    else {

    }
}

async function validateForm() {
    if (bookName !== undefined && authorName !== undefined && isbn !== undefined && globalBookId !== undefined) {
        if (bookName !== '' && authorName !== '' && isbn !== '') {

            //isUpdating = globalBookId === 0 ? false : true;

            return true;
        }
    }
    return false;
}

async function doesExist(b,i) {
    let isExists = false;
    $.each(booksArr, (key, value) => {
        if (value.bookName.toLowerCase() === b.toLowerCase()
            || value.isbn === i) {
            isExists = true;
            return;
        }
    });
    return isExists;
}

async function addRow() {

    doesExist(bookName,isbn).then(resp => {
        if (resp) {
            toastr.info('Book already exists!');
        }
        else {

            let table = document.querySelector("#myBooks tbody");

            let newBook = document.createElement("tr");
            newBook.setAttribute("id", `bookId_${0}`);
            //$(newBook).append(`<td>${globalBookId}</td>`);
            $(newBook).append(`<td>${bookName}</td>`);
            $(newBook).append(`<td>${authorName}</td>`);
            $(newBook).append(`<td>${isbn}</td>`);
            $(newBook).append(`<td class='action-flex'>
                    <i onclick="UpdateBook(this.parentElement.parentElement,${0},'${bookName}','${authorName}','${isbn}')" class="fas fa-edit"></i>
                    <i onclick="deleteBook(${globalBookId})" class="trash fas fa-trash-alt"></i>
                </td>`);


            table.append(newBook);

            document.getElementById("myBooks").scrollIntoView(false);
           
            let bookToAdd = {
                bookName: bookName,
                authorName: authorName,
                isbn: isbn
            };

            booksArr[`id_${globalBookId}`] = bookToAdd;


            let bookOperation = {
                operation: INSERT,
                book: {
                    id: globalBookId,
                    bookName: bookName,
                    authorName: authorName,
                    isbn: isbn
                }
            };

            operationsQ.push(bookOperation);

            globalBookId++;
        }
        clearForm();
    });
   
   
}



async function UpdateBook(row,bookId, bookName, authorName, isbn) {
    isUpdating = true;
    currentUpdatingRow = row;
    updatingBookId = bookId;
    $("#cancelUpdate").show();


    $("#upsertBtn").val("UPDATE");

    $("#bookId").val(bookId);
    $("#bookName").val(bookName);
    $("#authorName").val(authorName);
    $("#isbn").val(isbn);
}


async function clearForm() {
    //globalBookId = 0;
    bookName = undefined;
    authorName = undefined;
    isbn = undefined;
    updatingBookId = 0;

    $("#bookId").val('');
    $("#bookName").val('');
    $("#authorName").val('');
    $("#isbn").val('');
}

async function deleteBook(bookIdToDelete) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {

            if (bookIdToDelete === updatingBookId) {
                toastr.warning("Can't delete the book you're updating.<br>First Cancel Update.");
            } else {
                $(`#bookId_${bookIdToDelete}`).remove();

                delete booksArr[`id_${bookIdToDelete}`];

                Swal.fire(
                    'Deleted!',
                    'Book has been deleted.',
                    'success'
                )

                //adding operations
                let bookOperation = {
                    operation: DELETE,
                    book: {
                        id: bookIdToDelete
                    }
                };

                operationsQ.push(bookOperation);
            }

           
            //if (document.querySelector("#myBooks tbody").children.length === 0) {
            //    let emptyDiv = document.createElement("div")
            //    emptyDiv.style.background = "lightgrey";
            //    emptyDiv.innerText = "No Data Available.";
            //    emptyDiv.style.width = "100%";

            //    document.querySelector("#myBooks tbody").append(emptyDiv);
            //}
        }
    })

    //showing no rows..
    
    
}

function scrollSmoothToBottom(id) {
    var div = document.getElementById(id);
    console.log(div, $('#' + id));
    $('#' + id).animate({
        scrollTop: div.scrollHeight - div.clientHeight
    }, 500);
}

function fillBooksList() {
    $("#myBooks tbody").children().each((i, row) => {
        let book_id = $(row).attr('id').split('_')[1];

        //console.log($(row).children().eq(2).text());

        booksList.push({
            id: book_id,
            bookName: $(row).children().eq(0).text().trim(),
            authorName: $(row).children().eq(1).text().trim(),
            isbn: $(row).children().eq(2).text().trim()
        });
    });
}

$().ready(() => {

    document.getElementById("myBooks").scrollIntoView(false);

    //AJAX Loader
    $(document).on({
        ajaxStart: function () { $("body").addClass("loading"); },
        ajaxStop: function () { $("body").removeClass("loading"); }
    });

    $.ajax({
        type: 'get',
        url: 'BulkCrud/GetCurrentIdent',
        success: function (resp) {
            globalBookId = resp.currentId
            globalBookId++;
        }
    });

    /*globalBookId = Number(document.getElementById("myBooks").children[1].lastElementChild.children[0].innerText.trim());*/
    
    $("#saveToDbBtn").click(async (e) => {
        //Calling AJAX function to update database

        fillBooksList();

        //if (operationsQ.length > 0) {
            $.ajax({
                type: 'post',
                url: 'BulkCrud/SaveDatabaseBulk',
                data: {
                    obj: booksList
                    //obj: operationsQ
                },
                success: function (resp) {
                    toastr["success"]("Saved to database successfully.");
                },
                error: function (err) {
                    toastr["error"]("Error in server..<br>", err);
                }
            });
       // }
        booksList.splice(0, booksList.length);
       // operationsQ.splice(0, operationsQ.length);
    });

    let firstBook = {
        bookName: 'Harry potter 1',
        authorName: 'JK Rowling',
        isbn: '12841'
    };

    booksArr['id_1'] = firstBook;

    $("#cancelUpdate").click(e => {

        clearForm();

        $("#cancelUpdate").hide();

        isUpdating = false;
        $("#upsertBtn").val('ADD');

    });

    $("#upsertBtn").click((e) => {

        if (isUpdating) {
            setGlobalValues(true);
        }
        else {
            setGlobalValues(false);
        }
        validateForm().then((resp) => {
            //successfully validated..
            if (resp) {
                if (!isUpdating) {
                    addRow();
                   // 
                }
                //Updating..
                else {
                    //let rowToUpdate = document.getElementById(`bookId_${updatingBookId}`);
                    let rowToUpdate = currentUpdatingRow;       

                    //0  -  1    -  2    -  3
                    //id - bname - aname - isbn

                    rowToUpdate.children[0].innerText = bookName;
                    rowToUpdate.children[1].innerText = authorName;
                    rowToUpdate.children[2].innerText = isbn;

                    rowToUpdate.children[3].children[0].setAttribute('onclick', `UpdateBook(${updatingBookId},'${bookName}','${authorName}','${isbn}')`);

                    $(rowToUpdate).fadeOut(1000).fadeIn(500);

                    booksArr[`id_${updatingBookId}`] = {
                        bookName: bookName,
                        authorName: authorName,
                        isbn: isbn
                    }

                    let bookOperation = {
                        operation: UPDATE,
                        book: {
                            id: updatingBookId,
                            bookName: bookName,
                            authorName: authorName,
                            isbn: isbn
                        }
                    };

                    operationsQ.push(bookOperation);

                    isUpdating = false;
                    $("#upsertBtn").val("ADD");
                    $("#cancelUpdate").hide();
                    clearForm();
                }
            }
            //Validation is negative
            else {
                toastr.options = {
                    "preventDuplicates": true
                }

                toastr.error('All fields are mandatory.');
            }
        })

    });
})